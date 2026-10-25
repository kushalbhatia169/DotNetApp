# MyApp.Api

A .NET 10 GraphQL catalog API structured as four projects:

- `Domain` holds product and category entities and has no infrastructure dependencies.
- `Application` holds repository contracts and catalog use cases, including input/business validation.
- `Infrastructure` implements persistence with EF Core and SQLite. Provider-specific setup is isolated here so a server database can be introduced without changing GraphQL or application use cases.
- `MyApp.Api` hosts GraphQL, authentication, and the composition root.

SQLite is configured as the current provider at `Data/myapp.db`. SQLite is appropriate for a single-instance deployment with modest write concurrency; use a server database and its provider-specific migrations for multiple application instances or higher write throughput.

The application applies EF Core migrations at startup. On an empty database it imports the legacy `Data/categories.json` and `Data/products.json` files once; once SQLite contains data, those files are ignored. Back up the database before deploying schema changes.

## Configuration

Do not store the JWT signing key in source control. Configure `Jwt:Secret` through a secret manager or environment variable (`Jwt__Secret`), with at least 32 bytes of key material. For local development, use User Secrets:

```powershell
dotnet user-secrets set "Jwt:Secret" "<a private random secret of at least 32 bytes>" --project .\MyApp.Api.csproj
```

The `/get-test-token` endpoint is only mapped in Development. The previously committed signing key should be considered compromised and rotated.

## GraphQL operations

All queries require an authenticated user. Mutations require an authenticated user with the `Admin` role. Send the JWT as `Authorization: Bearer <token>`.

Product queries: `products`, `productById(id)`, `productsByIds(ids)`, and `productsByCategoryId(categoryId)`.

Category queries: `categories`, `categoryById(id)`, and `categoriesByIds(ids)`.

Product mutations: `addProduct(input)`, `updateProduct(productId, input)`, and `deleteProduct(productId)`.

Category mutations: `addCategory(input)`, `updateCategory(categoryId, input)`, and `deleteCategory(categoryId)`. Bulk category mutations `updateBulkCategory(categoryIds, input)` and `deleteBulkCategory(categoryIds)` are also available.

Example product query:

```graphql
query {
  products {
    id
    name
    price
    stock
    categoryId
    category { id name }
  }
}
```

Example product mutation:

```graphql
mutation {
  addProduct(input: {
    name: "Wireless keyboard"
    price: 1950
    categoryId: "103"
    stock: 8
  }) {
    id
    name
    price
    stock
    categoryId
  }
}
```

`updateProduct` accepts optional fields in its input and leaves omitted values unchanged. `updateCategory` currently requires a non-empty name.

## Build

```powershell
dotnet build .\MyApp.Api.slnx
dotnet ef migrations add <MigrationName> --project .\Infrastructure\MyApp.Infrastructure.csproj --startup-project .\MyApp.Api.csproj
```