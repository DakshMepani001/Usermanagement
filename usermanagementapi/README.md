# User Management API

ASP.NET Core Web API for CRUD operations on user records (in-memory persistence).

## Run the API

```powershell
cd .\UserManagementAPI
dotnet run
```

OpenAPI is available at `/openapi/v1.json` (shown when `ASPNETCORE_ENVIRONMENT=Development`).

Base route:
- `https://localhost:<port>/api/users`

## Authentication

All `api/*` endpoints require a bearer token.
Use `Authorization: Bearer dev-token-change-me` (configured in `appsettings.json` under `Auth:Token`).

## Endpoints

1. `GET /api/users` - list all users
2. `GET /api/users/{id}` - get a single user by id
3. `POST /api/users` - create a new user
4. `PUT /api/users/{id}` - update an existing user
5. `DELETE /api/users/{id}` - delete a user

## Quick Postman test flow

1. Add header `Authorization: Bearer dev-token-change-me` to all requests.
2. `POST /api/users` with body:
   ```json
   {
     "firstName": "Asha",
     "lastName": "Patel",
     "email": "asha.patel@example.com",
     "phone": "+15551234567"
   }
   ```
3. Copy the `id` from the `POST` response.
4. Use `GET /api/users/{id}` to verify retrieval.
5. Use `PUT /api/users/{id}` to update details.
6. Use `DELETE /api/users/{id}` to remove the user, then confirm with `GET`.

## Postman collection

Import `postman_collection.json` from this folder.

