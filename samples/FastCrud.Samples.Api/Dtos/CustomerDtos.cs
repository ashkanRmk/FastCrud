using System;

namespace FastCrud.Samples.Api.Dtos
{
    /// <summary>
    /// DTO used when creating a customer. All fields are required.
    /// </summary>
    public record CustomerCreateDto(string FirstName, string LastName, string Email);

    /// <summary>
    /// DTO used when updating a customer. All fields are required.
    /// </summary>
    public record CustomerUpdateDto(string FirstName, string LastName, string Email);

    /// <summary>
    /// DTO returned when reading a customer. Includes the identifier and timestamp.
    /// </summary>
    public record CustomerReadDto(Guid Id, string FirstName, string LastName, string Email, DateTime CreatedUtc);
}