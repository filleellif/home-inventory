# Home Inventory Frontend

A responsive Vue 3/Nuxt 3 frontend for the Home Inventory Management System.

## Tech Stack

- **Framework**: Nuxt 3 (Vue 3 with TypeScript)
- **Styling**: Tailwind CSS
- **UI Components**: Headless UI (@headlessui/vue) + custom components
- **Icons**: Heroicons (@heroicons/vue)
- **State Management**: Pinia for auth, composables for feature state
- **API Client**: Nuxt's built-in `useFetch` with custom wrapper

## Features

- **Dashboard**: Overview with statistics and recent items
- **Items Management**: Complete CRUD operations with pagination and filtering
- **Categories**: Hierarchical category management
- **Search & Filters**: Filter items by name, category, room
- **Auth UI**: Login and register pages (prepared for backend implementation)
- **Responsive Design**: Mobile-first design with Tailwind CSS

## Prerequisites

- Node.js 18+ and npm
- Backend API running at `http://localhost:5000/api`

## Installation

```bash
# Install dependencies
npm install
```

## Development

```bash
# Start dev server at http://localhost:3000
npm run dev
```

The frontend will automatically connect to the backend API at `http://localhost:5000/api` (configurable via `NUXT_PUBLIC_API_BASE` environment variable).

## Build

```bash
# Build for production
npm run build

# Preview production build
npm run preview
```

## Environment Variables

Create a `.env` file if you need to customize the API URL:

```env
NUXT_PUBLIC_API_BASE=http://localhost:5000/api
```

## Project Structure

```
frontend/
├── assets/css/           # Tailwind CSS
├── components/
│   ├── ui/              # Reusable base components
│   ├── items/           # Item-specific components
│   ├── categories/      # Category components
│   ├── auth/            # Auth forms
│   └── layout/          # Header, sidebar
├── composables/         # Reusable composition functions
├── layouts/             # Page layouts
├── pages/               # File-based routing
├── stores/              # Pinia stores
└── types/               # TypeScript type definitions
```

## Key Features

### Items Management
- View all items in a paginated table
- Create, edit, and delete items
- Rich item information (basic info, financial, location, tags)
- Filter by category, room, or search by name

### Categories
- Create categories with optional parent (hierarchy)
- View categories in tree structure
- Used for organizing items

### Dashboard
- Quick statistics (total items, categories, value)
- Recent items grid
- Quick action buttons

### Auth UI
- Login and register pages (ready for backend implementation)
- Form validation
- Integration with auth store

## Authentication

The frontend includes auth UI (login/register pages) that is prepared for future backend implementation. Currently, the backend doesn't have authentication, so the auth functions are mocked in the auth store.

When the backend implements authentication:
1. Update `stores/auth.ts` to call the actual auth API endpoints
2. Implement token storage in localStorage/cookies
3. Add middleware to protect routes

## API Integration

All API calls go through the `useApi` composable which:
- Configures base URL from runtime config
- Handles auth token injection
- Provides global error handling
- Returns properly typed responses

## TypeScript

The project uses strict TypeScript mode with type definitions that match the backend DTOs exactly. All API responses are properly typed for type safety.

## Tailwind CSS

The project uses Tailwind CSS with a custom color scheme (primary blue). The `@tailwindcss/forms` plugin is included for better form styling.

## Troubleshooting

### API Connection Issues
- Ensure the backend is running at `http://localhost:5000`
- Check CORS settings in the backend (should allow `localhost:3000`)
- Verify the API base URL in `nuxt.config.ts`

### Build Errors
- Run `npm run clean` (if available) or delete `.nuxt` directory
- Delete `node_modules` and `package-lock.json`, then run `npm install` again
- Ensure Node.js version is 18+

### Type Errors
- Run `npm run postinstall` to regenerate Nuxt types
- Check that all `~/types/*.ts` files are properly exported

## License

Copyright © 2024
