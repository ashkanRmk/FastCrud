# FastCrud — Package Split Skeleton

- **FastCrud.Abstractions** — contracts and primitives (no heavy deps)
- **FastCrud.Core** — CRUD orchestration over abstractions
- **FastCrud.Persistence.EFCore** — EF Core repository adapter
- **FastCrud.Mapping.Mapster** — Mapster adapter
- **FastCrud.Query.Gridify** — Gridify query adapter
- **FastCrud.Validation.FluentValidation** — FluentValidation adapter
- **FastCrud.Web.MinimalApi** — Minimal API endpoint mappings
- **FastCrud.Meta** — Batteries-included meta-package (depends on the above)
- **FastCrud.Samples.Api** — (not packed) sample usage with Minimal API
