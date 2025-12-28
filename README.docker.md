# Home Inventory - Docker Setup

This guide explains how to run the Home Inventory application using Docker.

## Prerequisites

- Docker Desktop (or Docker Engine + Docker Compose)
- At least 4GB of RAM available for Docker
- Ports 3000, 5000, and 5432 available on your host machine

## Quick Start

### 1. Clone the repository (if you haven't already)

```bash
git clone <repository-url>
cd home-inventory
```

### 2. Build and start all services

```bash
docker compose up --build
```

This command will:
- Build the backend .NET application
- Build the frontend Nuxt application
- Pull the PostgreSQL database image
- Start all services with proper networking
- Run database migrations automatically

### 3. Access the application

- **Frontend**: http://localhost:3000
- **Backend API**: http://localhost:5000/api
- **Swagger UI**: http://localhost:5000/swagger
- **PostgreSQL**: localhost:5432

## Commands

### Start services (with build)
```bash
docker compose up --build
```

### Start services in detached mode (background)
```bash
docker compose up -d
```

### Stop services
```bash
docker compose down
```

### Stop services and remove volumes (data will be lost)
```bash
docker compose down -v
```

### View logs
```bash
# All services
docker compose logs -f

# Specific service
docker compose logs -f backend
docker compose logs -f frontend
docker compose logs -f postgres
```

### Rebuild a specific service
```bash
# Backend only
docker compose up --build backend

# Frontend only
docker compose up --build frontend
```

### Access a running container
```bash
# Backend
docker compose exec backend /bin/bash

# Frontend
docker compose exec frontend /bin/sh

# Database
docker compose exec postgres psql -U postgres -d homeinventory
```

## Service Details

### Backend (API)
- **Image**: Built from `./backend/Dockerfile`
- **Port**: 5000 → 8080 (container)
- **Dependencies**: PostgreSQL
- **Health**: Automatically waits for database to be ready

### Frontend
- **Image**: Built from `./frontend/Dockerfile`
- **Port**: 3000 → 3000 (container)
- **Dependencies**: Backend API
- **Framework**: Nuxt 4

### Database (PostgreSQL)
- **Image**: postgres:17-alpine
- **Port**: 5432 → 5432 (container)
- **Default Credentials**:
  - Database: `homeinventory`
  - User: `postgres`
  - Password: `postgres`
- **Data**: Persisted in Docker volume `postgres_data`

## Environment Variables

You can customize the configuration by creating a `.env` file in the root directory. See `.env.example` for available options.

Example `.env`:
```env
POSTGRES_PASSWORD=your-secure-password
BACKEND_PORT=5000
FRONTEND_PORT=3000
```

## Database Migrations

The backend automatically applies Entity Framework migrations on startup. If you need to run migrations manually:

```bash
# Access backend container
docker compose exec backend /bin/bash

# Run migrations
dotnet ef database update --project /src/src/HomeInventory.Infrastructure --startup-project /src/src/HomeInventory.WebApi
```

## Troubleshooting

### Backend fails to start
- Check if PostgreSQL is healthy: `docker compose logs postgres`
- Ensure port 5000 is not in use: `lsof -i :5000` (macOS/Linux)
- Check backend logs: `docker compose logs backend`

### Frontend can't connect to backend
- Verify backend is running: `curl http://localhost:5000/api/health` (if health endpoint exists)
- Check CORS settings in backend configuration
- Review frontend logs: `docker compose logs frontend`

### Database connection issues
- Ensure PostgreSQL is running: `docker compose ps postgres`
- Check database logs: `docker compose logs postgres`
- Verify connection string in backend environment variables

### Port conflicts
If default ports are in use, modify the port mappings in `docker-compose.yml`:

```yaml
services:
  backend:
    ports:
      - "5001:8080"  # Change 5000 to 5001
  frontend:
    ports:
      - "3001:3000"  # Change 3000 to 3001
```

## Production Deployment

For production deployment, consider:

1. **Use secrets** instead of plain text passwords
2. **Enable HTTPS** with reverse proxy (nginx/Traefik)
3. **Set proper CORS** origins for your domain
4. **Use persistent volumes** for database backups
5. **Configure logging** and monitoring
6. **Set resource limits** for containers

Example production adjustments:

```yaml
services:
  backend:
    environment:
      ASPNETCORE_ENVIRONMENT: Production
      ConnectionStrings__DefaultConnection: "${DATABASE_URL}"
    deploy:
      resources:
        limits:
          cpus: '1'
          memory: 1G
```

## Cleanup

To completely remove all containers, volumes, and networks:

```bash
# Stop and remove containers, networks, and volumes
docker compose down -v

# Remove built images
docker rmi home-inventory-backend home-inventory-frontend

# Prune unused Docker resources (optional)
docker system prune -a
```
