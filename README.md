# FastCrud — Package Split Skeleton

This folder contains a ready-to-merge split of **FastCrud** into composable NuGet packages:

- **FastCrud.Abstractions** — contracts and primitives (no heavy deps)
- **FastCrud.Core** — CRUD orchestration over abstractions
- **FastCrud.Persistence.EFCore** — EF Core repository adapter
- **FastCrud.Mapping.Mapster** — Mapster adapter
- **FastCrud.Query.Gridify** — Gridify query adapter
- **FastCrud.Validation.FluentValidation** — FluentValidation adapter
- **FastCrud.Web.MinimalApi** — Minimal API endpoint mappings
- **FastCrud.Meta** — Batteries-included meta-package (depends on the above)
- **FastCrud.Samples.Api** — (not packed) sample usage with Minimal API

> No sample entities are packaged. Adapters hide 3rd-party libraries behind interfaces.

## Quick start (merge into repo root)

```bash
git checkout -b chore/split-fastcrud
# Unzip next to your existing repo files
unzip fastcrud-split.zip -d .

# Create solution and add projects (if you don't already have one):
dotnet new sln -n FastCrud || true
dotnet sln FastCrud.sln add src/**/**.csproj

# Build
dotnet restore
dotnet build -c Release

# Pack all
dotnet pack -c Release
```

## CI publish (NuGet)
- Add secret `NUGET_API_KEY` to your repo.
- Tag a release like `v1.0.0` → workflow builds and pushes packages.

Adjust **Directory.Packages.props** versions if needed.
