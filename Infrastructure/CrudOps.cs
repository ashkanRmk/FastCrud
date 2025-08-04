namespace Crud.Generator.Infrastructure;

[Flags]
public enum CrudOps
{
    None = 0,
    GetAll = 1 << 0,
    GetById = 1 << 1,
    Create = 1 << 2,
    Update = 1 << 3,
    Delete = 1 << 4,
    All = GetAll | GetById | Create | Update | Delete
}