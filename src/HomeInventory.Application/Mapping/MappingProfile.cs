using AutoMapper;
using HomeInventory.Application.DTOs;
using HomeInventory.Domain.Aggregates.CategoryAggregate;
using HomeInventory.Domain.Aggregates.InventoryItemAggregate;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // InventoryItem mappings
        CreateMap<InventoryItem, ItemDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.BasicInfo.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.BasicInfo.Description))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.BasicInfo.Quantity))
            .ForMember(dest => dest.PurchasePrice, opt => opt.MapFrom(src => src.FinancialInfo.PurchasePrice != null ? src.FinancialInfo.PurchasePrice.Amount : (decimal?)null))
            .ForMember(dest => dest.PurchaseCurrency, opt => opt.MapFrom(src => src.FinancialInfo.PurchasePrice != null ? src.FinancialInfo.PurchasePrice.Currency : null))
            .ForMember(dest => dest.CurrentValue, opt => opt.MapFrom(src => src.FinancialInfo.CurrentValue != null ? src.FinancialInfo.CurrentValue.Amount : (decimal?)null))
            .ForMember(dest => dest.CurrentValueCurrency, opt => opt.MapFrom(src => src.FinancialInfo.CurrentValue != null ? src.FinancialInfo.CurrentValue.Currency : null))
            .ForMember(dest => dest.PurchaseDate, opt => opt.MapFrom(src => src.FinancialInfo.PurchaseDate))
            .ForMember(dest => dest.Room, opt => opt.MapFrom(src => src.Location.Room))
            .ForMember(dest => dest.StorageSpot, opt => opt.MapFrom(src => src.Location.StorageSpot))
            .ForMember(dest => dest.GpsLatitude, opt => opt.MapFrom(src => src.Location.Coordinates != null ? src.Location.Coordinates.Latitude : (double?)null))
            .ForMember(dest => dest.GpsLongitude, opt => opt.MapFrom(src => src.Location.Coordinates != null ? src.Location.Coordinates.Longitude : (double?)null))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId != null ? src.CategoryId.Value : (Guid?)null))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => t.Value).ToList()))
            .ForMember(dest => dest.Photos, opt => opt.MapFrom(src => src.Photos))
            .ForMember(dest => dest.Receipts, opt => opt.MapFrom(src => src.Receipts));

        // Category mappings
        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.ParentCategoryId, opt => opt.MapFrom(src => src.ParentCategoryId != null ? src.ParentCategoryId.Value : (Guid?)null));

        // MediaReference mappings
        CreateMap<MediaReference, MediaReferenceDto>()
            .ForMember(dest => dest.MediaType, opt => opt.MapFrom(src => src.MediaType.ToString()));
    }
}
