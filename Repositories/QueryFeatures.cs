namespace Crud.Generator.Repositories;

[Flags]
public enum QueryFeatures
{
    None   = 0,
    Filter = 1 << 0,
    Sort   = 1 << 1,
    Page   = 1 << 2,
    All    = Filter | Sort | Page
}