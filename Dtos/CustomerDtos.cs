namespace Crud.Generator.Dtos;

public sealed class CustomerReadDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}

public sealed class CustomerCreateDto
{
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
}

public sealed class CustomerUpdateDto
{
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
}