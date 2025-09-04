using FastCrud.Samples.Api.Models;
using FluentValidation;

namespace FastCrud.Samples.Api.Validators
{
    /// <summary>
    /// Validator for <see cref="Order"/> entities. Validates order number and non-negative amount.
    /// </summary>
    public class OrderValidator : AbstractValidator<Order>
    {
        public OrderValidator()
        {
            RuleFor(x => x.Number).NotEmpty();
            RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
        }
    }
}