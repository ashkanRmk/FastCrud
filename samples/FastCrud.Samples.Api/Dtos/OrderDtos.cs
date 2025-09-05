using System;

namespace FastCrud.Samples.Api.Dtos
{
    public record OrderCreateDto(Guid CustomerId, string Number, decimal Amount);

    public record OrderUpdateDto(Guid CustomerId, string Number, decimal Amount);

    public record OrderReadDto(Guid Id, Guid CustomerId, string Number, decimal Amount, DateTime PlacedUtc);
}