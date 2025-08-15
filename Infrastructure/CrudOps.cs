namespace FastCrud.Infrastructure;

[Flags]
public enum CrudOps
{
    None = 0,
    GetAll = 1 << 0,
    GetById = 1 << 1,
    Create = 1 << 2,
    Update = 1 << 3,
    Delete = 1 << 4,
    GetPaginated = 1 << 5,
    GetFiltered = 1 << 6,
    GetSorted = 1 << 7,
    GetFullOpsList = GetPaginated | GetFiltered | GetSorted,
    AllOps = GetAll | GetById | Create | Update | Delete | GetFullOpsList
}