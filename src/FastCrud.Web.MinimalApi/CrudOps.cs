namespace FastCrud.Web.MinimalApi;

[Flags]
public enum CrudOps
{
    None = 0,
    GetList = 1 << 0,
    GetById = 1 << 1,
    Create = 1 << 2,
    Update = 1 << 3,
    Delete = 1 << 4,
    AllOps = GetList | GetById | Create | Update | Delete
}