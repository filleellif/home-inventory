using HomeInventory.Application.Commands;
using HomeInventory.Application.Commands.Areas.CreateArea;
using HomeInventory.Application.Commands.Areas.DeleteArea;
using HomeInventory.Application.Commands.Areas.UpdateArea;
using HomeInventory.Queries.Interfaces;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

namespace HomeInventory.WebApi.Controllers.Areas;

[ApiController]
[Route("api/[controller]")]
public class AreasController(
    IAreaQueries areaQueries,
    ILogger<AreasController> logger) : ControllerBase
{
    /// <summary>
    /// Get all areas
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        logger.LogInformation("Fetching all areas");

        var areas = await areaQueries.GetAllAsync();

        return Ok(areas);
    }

    /// <summary>
    /// Get a specific area by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        logger.LogInformation("Fetching area with ID: {AreaId}", id);

        var area = await areaQueries.GetByIdAsync(id);

        if (area == null)
        {
            return NotFound(new { message = "Area not found" });
        }

        return Ok(area);
    }

    /// <summary>
    /// Create a new area
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateAreaRequest request,
        [FromServices] ICommandHandler<CreateAreaCommand> handler)
    {
        logger.LogInformation("Creating new area: {AreaName}", request.Name);

        var areaId = Guid.NewGuid();
        var command = request.ToCommand(areaId);

        await handler.HandleAsync(command);

        return CreatedAtAction(nameof(GetById), new { id = areaId }, new { id = areaId });
    }

    /// <summary>
    /// Update an existing area
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateAreaRequest request,
        [FromServices] ICommandHandler<UpdateAreaCommand> handler)
    {
        logger.LogInformation("Updating area with ID: {AreaId}", id);

        var command = request.ToCommand(id);

        await handler.HandleAsync(command);

        return Ok(new { message = "Area updated successfully" });
    }

    /// <summary>
    /// Delete an area
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        [FromServices] ICommandHandler<DeleteAreaCommand> handler)
    {
        logger.LogInformation("Deleting area with ID: {AreaId}", id);

        var command = new DeleteAreaCommand(id);
        await handler.HandleAsync(command);

        return NoContent();
    }

    /// <summary>
    /// Generate a printable QR code label for an area
    /// </summary>
    [HttpGet("qr-label/{qrCode}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetQrLabel(string qrCode, [FromQuery] string? label = null)
    {
        logger.LogInformation("Generating QR label for code: {QrCode}", qrCode);

        try
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(qrCode, QRCodeGenerator.ECCLevel.Q);
            using var pngQrCode = new PngByteQRCode(qrCodeData);
            var qrCodeImage = pngQrCode.GetGraphic(20);

            // If no label is provided, just return the QR code
            if (string.IsNullOrWhiteSpace(label))
            {
                return File(qrCodeImage, "image/png");
            }

            // Create a label image with the QR code and text
            using var ms = new MemoryStream(qrCodeImage);
            using var qrBitmap = new Bitmap(ms);

            var labelHeight = 60;
            var totalHeight = qrBitmap.Height + labelHeight;
            using var labelBitmap = new Bitmap(qrBitmap.Width, totalHeight);
            using var graphics = Graphics.FromImage(labelBitmap);

            // White background
            graphics.Clear(Color.White);

            // Draw QR code
            graphics.DrawImage(qrBitmap, 0, 0);

            // Draw label text
            using var font = new Font("Arial", 12, FontStyle.Bold);
            using var brush = new SolidBrush(Color.Black);
            var textFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var textRect = new RectangleF(0, qrBitmap.Height, qrBitmap.Width, labelHeight);
            graphics.DrawString(label, font, brush, textRect, textFormat);

            // Save to memory stream
            using var outputMs = new MemoryStream();
            labelBitmap.Save(outputMs, ImageFormat.Png);
            var imageBytes = outputMs.ToArray();

            return File(imageBytes, "image/png");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to generate QR label for code: {QrCode}", qrCode);
            return BadRequest(new { message = "Failed to generate QR label" });
        }
    }
}
