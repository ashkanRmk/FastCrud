using System;

namespace FastCrud.Samples.Api.Dtos
{
    /// <summary>
    /// DTO used when creating an order.
    /// </summary>
    public record OrderCreateDto(Guid CustomerId, string Number, decimal Amount);

    /// <summary>
    /// DTO used when updating an order.
    /// </summary>
    public record OrderUpdateDto(Guid CustomerId, string Number, decimal Amount);

    /// <summary>
    /// DTO returned when reading an order.
    /// </summary>
    public record OrderReadDto(Guid Id, Guid CustomerId, string Number, decimal Amount, DateTime PlacedUtc);
}