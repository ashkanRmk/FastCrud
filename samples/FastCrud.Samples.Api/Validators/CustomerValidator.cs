using FastCrud.Samples.Api.Dtos;
using FluentValidation;

namespace FastCrud.Samples.Api.Validators
{
    public class CustomerValidator : AbstractValidator<CustomerCreateDto>
    {
        public CustomerValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required.");
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }
}