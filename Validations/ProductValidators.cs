using Crud.Generator.Dtos;
using FluentValidation;

namespace Crud.Generator.Validations;

public sealed class ProductCreateValidator : AbstractValidator<ProductCreateDto>
{
    public ProductCreateValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
    }
}

public sealed class ProductUpdateValidator : AbstractValidator<ProductUpdateDto>
{
    public ProductUpdateValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
    }
}