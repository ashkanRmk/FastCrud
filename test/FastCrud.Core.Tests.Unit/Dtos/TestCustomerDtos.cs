namespace FastCrud.Core.Tests.Unit.Dtos;

public record TestCustomerCreateDto(string FirstName, string LastName, string Email);

public record TestCustomerUpdateDto(string FirstName, string LastName, string Email);

public record TestCustomerReadDto(Guid Id, string FirstName, string LastName, string Email);
