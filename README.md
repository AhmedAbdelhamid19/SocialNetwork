# SocialNetwork

A full-stack social networking application built with ASP.NET Core 8 Web API and Angular 20 (standalone components with signals). Features user authentication, member discovery, messaging, photo management, and dynamic theming.

## Features

### Authentication and Authorization
- User Registration (2-step form: credentials and profile)
- JWT-based Login and Logout
- Role-based Access Control
- Session Persistence (localStorage recovery on app reload)

### User Profiles
- Member Discovery (browse other users with filters)
- Profile Management (view and edit member information, bio, location)
- Age Calculation (auto-calculated from date of birth)
- Photo Gallery (upload, delete, set main photo)
- Pagination (configurable page size: 5, 10, 20, 50)

### Messaging System
- Direct Messaging (Inbox, Outbox, Unread tabs)
- Real-time Message Status (sent, received, read)
- Pagination and Filtering (by container type)
- Message Deletion (with cache invalidation)

### User Interface and User Experience
- Multi-theme Support (21+ DaisyUI themes: light, dark, cyberpunk, dracula, etc.)
- Global Loading Indicator (busy state across all HTTP requests)
- Toast Notifications (success, error, warning, info)
- Responsive Design (Tailwind CSS and DaisyUI)
- Smooth Route Animations (view transitions)
- Splash Screen (during app initialization)

### Data Management
- Smart HTTP Caching (GET requests cached in-memory, invalidated on mutations)
- Global Error Handling (HTTP interceptor with status-code-specific responses)
- Validation Error Display (inline form validation and server-side error messages)
- Seed Data (20+ demo users in database)

---

## Prerequisites

- .NET 8 SDK (backend)
- SQL Server (LocalDB or full SQL Server instance)
- Node.js 18+ and npm (frontend)
- Angular CLI version 20 or higher (npm install -g @angular/cli)

---

## Project Structure

```
SocialNetwork/
├── API/                          # ASP.NET Core 8 Web API
│   ├── Controllers/              # API endpoints (Auth, Members, Messages, Photos)
│   ├── Data/                     # Entity Framework DbContext, migrations
│   ├── DTOs/                     # Data Transfer Objects
│   ├── Entities/                 # Domain models (User, Message, Photo, etc.)
│   ├── Services/                 # Business logic (auth, member, message services)
│   ├── Helpers/                  # Utilities (JWT, pagination, etc.)
│   ├── Interfaces/               # Service and repository interfaces
│   ├── Middleware/               # Exception handling middleware
│   └── Program.cs                # App configuration, middleware, services
│
└── client/                        # Angular 20 Standalone App
    ├── src/
    │   ├── app/
    │   │   ├── app.config.ts     # App configuration (providers, interceptors)
    │   │   └── app.routes.ts     # Route definitions
    │   ├── core/
    │   │   ├── services/         # Singletons (AccountService, MessageService, etc.)
    │   │   ├── interceptors/     # HTTP interceptors (JWT, error, caching, loading)
    │   │   └── pipes/            # Custom pipes (age, timeAgo)
    │   ├── features/
    │   │   ├── account/          # Login, register components
    │   │   ├── members/          # Member list, detail, edit, photos
    │   │   ├── messages/         # Messaging UI
    │   │   └── home/             # Landing page
    │   ├── shared/               # Reusable components (TextInput, Paginator, etc.)
    │   ├── layout/               # Navigation, footer, theme system
    │   ├── types/                # TypeScript types and interfaces
    │   └── index.html            # Entry point (with splash screen)
    └── tailwind.config.js        # Tailwind and DaisyUI configuration
```

---

## Installation and Setup

### Backend Setup

#### Step 1: Configure Database Connection

Navigate to the API directory and open `appsettings.json`. Update the connection string to match your SQL Server instance:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=SocialNetwork;Trusted_Connection=True;Trust Server Certificate=True"
  }
}
```

For full SQL Server instances, replace the server name accordingly. For example:
- LocalDB: `(localdb)\MSSQLLocalDB`
- Named instance: `SERVER_NAME\INSTANCE_NAME`
- Default instance: `SERVER_NAME`

#### Step 2: Configure JWT Secret

The API requires a JWT signing key. Set this via User Secrets (recommended for development):

```bash
cd API
dotnet user-secrets init
dotnet user-secrets set "PrivateKey" "your-strong-random-secret-key-here-minimum-32-characters"
```

Alternatively, add to `API/appsettings.Development.json`:

```json
{
  "PrivateKey": "your-secret-key-here"
}
```

Note: For production, use environment variables or Azure Key Vault.

#### Step 3: Apply Database Migrations

Create the database and seed it with demo data:

```bash
cd API
dotnet ef database update
```

This command applies all pending migrations and populates the database with 20+ demo users from `API/Data/UserSeedData.json`.

#### Step 4: Run the Backend Server

```bash
dotnet run --project API
```

The API will start on `https://localhost:5001`. Verify it is running by navigating to `https://localhost:5001/api/` (you should see a welcome response or 404 for the base API path).

API endpoints are available at: `https://localhost:5001/api/`

CORS is configured to allow: `http://localhost:4200`

---

### Frontend Setup

#### Step 1: Navigate to Client Directory

```bash
cd client
```

#### Step 2: Install Node Dependencies

```bash
npm install
```

This installs all required packages including Angular, RxJS, Tailwind CSS, and DaisyUI.

#### Step 3: Verify API Base URL Configuration

Open `client/src/core/services/account-service.ts` and verify the base URL matches your backend:

```typescript
private baseUrl = 'https://localhost:5001/api/';
```

If your API runs on a different URL or port, update this value accordingly.

#### Step 4: Start Development Server

```bash
ng serve
```

The Angular development server will start on `http://localhost:4200`. The application will automatically reload when you modify any files.

#### Step 5: Open Application in Browser

Navigate to `http://localhost:4200` in your web browser.

---

## Running the Complete Application

### Quick Start

To start both backend and frontend simultaneously, use separate terminal windows:

```bash
# Terminal 1: Start Backend
cd API
dotnet run

# Terminal 2: Start Frontend
cd client
ng serve
```

Then open `http://localhost:4200` in your browser.

### Demo Credentials

Use one of the seeded demo users to test the application. Credentials are available in the seed data. Example:

- Username: demo
- Password: password

(Check `API/Data/UserSeedData.json` for complete list)

---

## Architecture and Key Features

### Architecture Overview

#### Standalone Components and Signals (Angular 20)
- All components are standalone (no NgModule boilerplate required)
- Signals for reactive state management (currentUser, validationErrors, etc.)
- Computed properties for derived state calculations

#### HTTP Interceptor Pipeline

Every HTTP request runs through the following interceptors in order:

1. **errorInterceptor** - Catches errors and handles them based on HTTP status codes, shows toasts, and navigates to error pages
2. **jwtInterceptor** - Attaches JWT authentication token to Authorization header
3. **cachingInterceptor** - Caches GET request responses, invalidates cache on POST/PUT/DELETE operations
4. **loadingInterceptor** - Toggles global busy state (loading spinner visibility)

#### Smart HTTP Caching

- In-memory Map caches GET responses with deterministic keys (URL and sorted query parameters)
- Cache is automatically invalidated on mutating requests (POST, PUT, DELETE) to prevent stale data display
- Cached responses return instantly, bypassing the loading state for improved perceived performance
- Can be extended with Time-To-Live (TTL) or Least Recently Used (LRU) eviction policies

#### Application Initialization Flow

1. Application boots and displays splash screen with "Loading..." message
2. Angular runs APP_INITIALIZER provider which calls InitService.init()
3. Service checks localStorage for persisted user session
4. If user exists, loads current user into AccountService (global state)
5. Splash screen is removed from DOM
6. Application components render
7. Router loads initial route based on user authentication status

#### Error Handling

The error interceptor handles different HTTP status codes as follows:

| Status Code | Behavior |
|-------------|----------|
| 400 | Extracts validation errors from response, displays in component form |
| 401 | Shows "Unauthorized" toast notification |
| 404 | Navigates to `/not-found` page |
| 500 | Navigates to `/server-error` page with error details |
| Other | Shows generic "Something went wrong" toast notification |

#### Registration (2-Step Form)

Step 1 - Credentials:
- Email (required, valid email format)
- Display Name (required)
- Password (required, 4-8 characters)
- Confirm Password (required, must match password)

Step 2 - Profile:
- Gender (male or female radio buttons)
- Date of Birth (date picker)
- City (text input)
- Country (text input)

Features:
- Custom validator for password confirmation matching
- Server-side validation error display inline
- Confirm Password field removed before API submission
- Form navigation with Next and Back buttons
- Disabled buttons when form is invalid

#### Messaging System

- Three-tab interface: Inbox, Outbox, Unread
- Pagination (5 items per page, configurable)
- Time-ago pipe for relative timestamps ("5 minutes ago", "Just now", etc.)
- Delete message functionality with automatic cache invalidation

#### Photo Management

- Upload main profile photo and gallery photos
- Set or delete photos
- Gallery carousel view
- Image URLs stored in database
- Validation and error handling

#### Dynamic Theming

- Support for 21 DaisyUI themes: light, dark, cyberpunk, dracula, forest, garden, lemonade, lofi, luxury, nord, pastel, retro, sunset, synthwave, and more
- Theme persisted in localStorage
- Dropdown selector in navbar for easy theme switching
- Applied via data-theme attribute on HTML element

---

## API Endpoints

Import `Social Network App.postman_collection.json` for complete endpoint reference.

### Authentication
- `POST /api/account/register` - Register new user
- `POST /api/account/login` - Login, returns JWT token

### Members
- `GET /api/members` - List members (with filters: gender, age range, ordering)
- `GET /api/members/:username` - Get member detail
- `PUT /api/members/:username` - Update member profile

### Photos
- `POST /api/photos` - Upload photo
- `DELETE /api/photos/:photoId` - Delete photo

### Messages
- `GET /api/messages` - Get messages (Inbox/Outbox/Unread)
- `POST /api/messages` - Send message
- `DELETE /api/messages/:messageId` - Delete message

---

## Technology Stack

### Backend
- ASP.NET Core 8 - Web framework
- Entity Framework Core - ORM
- JWT - Authentication
- AutoMapper - DTO mapping
- SQL Server - Database

### Frontend
- Angular 20 - UI framework (standalone components)
- RxJS - Reactive programming
- Tailwind CSS - Styling utility framework
- DaisyUI - Component library with themes
- TypeScript - Type-safe JavaScript
- Reactive Forms - Form management

---

## Troubleshooting

### SQL Connection Errors
- Verify connection string in `appsettings.json` matches your SQL Server instance
- For LocalDB: instance name is typically `(localdb)\MSSQLLocalDB`
- Ensure SQL Server is running and accessible
- Check Windows Authentication settings if using integrated security

### 401 Unauthorized Errors
- Confirm `PrivateKey` is set in User Secrets or `appsettings.Development.json`
- Client JWT token may be expired - logout and login again
- Check that `Authorization: Bearer <token>` header is sent in requests
- Verify token was properly stored in localStorage after login

### CORS Errors
- API CORS policy allows `http://localhost:4200` by default
- If Angular runs on different port, update CORS in `API/Program.cs`
- Check browser console for specific CORS error messages

### HTTPS/SSL Issues
- Browser may block localhost HTTPS without trusted certificate
- On Windows, trust the dev certificate:
  ```bash
  dotnet dev-certs https --trust
  ```
- On macOS/Linux, follow similar steps or accept browser warning
- Verify API URL uses correct protocol (https vs http)

### API Not Responding
- Verify API is running: `dotnet run --project API`
- Check API URL in `client/src/core/services/account-service.ts`
- Browser DevTools Network tab shows actual requests and responses
- Check API console output for error messages
- Ensure firewall allows connections to port 5001

### Caching Issues
- Clear browser cache and localStorage if stale data appears
- Restart development servers (both backend and frontend)
- Check Network tab in DevTools to verify cache behavior
- Invalid cache entries are cleared on mutating requests (POST/PUT/DELETE)

---

## Development Notes and Future Improvements

### Caching Interceptor
- Current implementation: in-memory Map with no size limit
- TODO: Add Time-To-Live (TTL) per cache entry with automatic expiration
- TODO: Implement LRU (Least Recently Used) eviction when cache exceeds maximum size
- TODO: Store only response bodies instead of full HttpEvent to reduce memory usage
- TODO: Add cache statistics and monitoring for debugging

### Error Handling
- Current: global interceptor handles all HTTP errors with status-code routing
- TODO: Add retry logic for transient failures (429 Too Many Requests, 503 Service Unavailable)
- TODO: Implement exponential backoff for retry mechanism
- TODO: Add offline detection and request queue for offline-first functionality

### Performance Optimization
- Current: no lazy loading of feature routes
- TODO: Lazy load member detail, messages, and other feature modules
- TODO: Implement virtual scrolling for large member lists to reduce DOM elements
- TODO: Add route preloading strategy for improved perceived performance
- TODO: Implement image optimization and lazy loading for photo gallery

### Testing
- TODO: Unit tests for services (AccountService, MessageService, MemberService)
- TODO: Unit tests for HTTP interceptors and their behavior
- TODO: Integration tests for authentication flow
- TODO: End-to-end tests for complete user workflows (registration, messaging, etc.)
- TODO: Component tests for form validation and error display

### Authentication Security
- TODO: Implement refresh token rotation for improved JWT security
- TODO: Add CSRF protection for form submissions
- TODO: Implement rate limiting on authentication endpoints
- TODO: Add two-factor authentication (2FA) support

### Features to Implement
- TODO: Follow/Unfollow user functionality
- TODO: Search functionality for members
- TODO: User notifications system
- TODO: Real-time messaging with SignalR (WebSockets)
- TODO: User activity tracking (last online, typing indicators)
- TODO: Advanced member filtering and search
- TODO: User profile privacy settings

---

## Resources

- Angular Documentation: https://angular.dev
- ASP.NET Core Documentation: https://docs.microsoft.com/aspnet/core
- DaisyUI Themes: https://daisyui.com/docs/themes/
- Tailwind CSS: https://tailwindcss.com
- RxJS: https://rxjs.dev
- Entity Framework Core: https://docs.microsoft.com/ef/core/

---

## License

This project is for educational purposes.

3) Start the dev server
```bash
ng serve
```
Open `http://localhost:4200`.

## Useful Notes
- Postman collection: import `Social Network App.postman_collection.json` to test endpoints.
- CORS: configured in API to allow Angular dev origin(s). If your port/origin differs, update the CORS policy in `Program.cs`.
- HTTPS dev cert: if you see SSL trust issues on Windows, run:
  ```bash
  dotnet dev-certs https --trust
  ```

## Troubleshooting
- SQL connection errors: ensure the connection string matches your SQL Server instance and that the database is reachable.
- 401 Unauthorized: confirm `PrivateKey` is configured and the client points to the correct API URL.
- CORS errors: update the allowed origins in `API/Program.cs` to include your Angular URL.

## Scripts Quickstart
```bash
# Backend
dotnet ef database update --project API
dotnet run --project API

# Frontend
cd client && npm i && ng serve
```
