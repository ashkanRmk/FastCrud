using System;

namespace FastCrud.Samples.Api.Dtos
{
    public record CustomerCreateDto(string FirstName, string LastName, string Email, List<TagDto> Tags);

    public record CustomerUpdateDto(string FirstName, string LastName, string Email);

    public record CustomerReadDto(Guid Id, string FirstName, string LastName, string Email, List<TagDto> Tags);
    
    public record TagDto(string Name, string Description);
}

