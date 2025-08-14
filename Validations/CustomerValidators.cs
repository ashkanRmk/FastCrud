using Crud.Generator.Dtos;
using FluentValidation;

namespace Crud.Generator.Validations;

public sealed class CustomerCreateValidator : AbstractValidator<CustomerCreateDto>
{
    public CustomerCreateValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}

public sealed class CustomerUpdateValidator : AbstractValidator<CustomerUpdateDto>
{
    public CustomerUpdateValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}