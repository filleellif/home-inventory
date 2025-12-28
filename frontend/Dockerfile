# Build stage
FROM node:22-alpine AS build
WORKDIR /app

# Copy package files
COPY package*.json ./

# Install dependencies
RUN npm ci

# Copy source code
COPY . .

# Build the application
RUN npm run build

# Runtime stage
FROM node:22-alpine AS runtime
WORKDIR /app

# Copy built application from build stage
COPY --from=build /app/.output /app/.output
COPY --from=build /app/package*.json /app/

# Set environment to production
ENV NODE_ENV=production

# Expose the port
EXPOSE 3000

# Start the application
CMD ["node", ".output/server/index.mjs"]
