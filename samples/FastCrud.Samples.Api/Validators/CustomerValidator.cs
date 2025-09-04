using FastCrud.Samples.Api.Models;
using FluentValidation;

namespace FastCrud.Samples.Api.Validators
{
    /// <summary>
    /// Validator for <see cref="Customer"/> entities. Ensures required fields are populated and email format is correct.
    /// </summary>
    public class CustomerValidator : AbstractValidator<Customer>
    {
        public CustomerValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }
}