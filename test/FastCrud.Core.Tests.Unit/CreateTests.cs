using FastCrud.Abstractions.Abstractions;
using FastCrud.Abstractions.Query;
using FastCrud.Core.Services;
using FastCrud.Core.Tests.Unit.Dtos;
using FastCrud.Core.Tests.Unit.Models;
using NSubstitute;

namespace FastCrud.Core.Tests.Unit;

public class CreateTests
{
    // Service Under Test
    private readonly CrudService<TestCustomer, Guid, 
        TestCustomerCreateDto, TestCustomerUpdateDto> _service;

    // Dependencies
    private readonly IRepository<TestCustomer, Guid> _repository 
        = Substitute.For<IRepository<TestCustomer, Guid>>();
    private readonly IObjectMapper _mapper
        = Substitute.For<IObjectMapper>();
    private readonly IEnumerable<IModelValidator<TestCustomer>> _validators
        = new List<IModelValidator<TestCustomer>>();
    private readonly IServiceProvider _serviceProvider
        = Substitute.For<IServiceProvider>();
    private readonly IQueryEngine _queryEngine
        = Substitute.For<IQueryEngine>();
    
    public CreateTests()
    {
        _service = new CrudService<TestCustomer, Guid, TestCustomerCreateDto, TestCustomerUpdateDto>
            (_repository, _mapper, _validators, _serviceProvider, _queryEngine);
    }
    
    [Fact]
    public async Task Create_ShouldThrowArgumentException_WhenInputIsNull()
    {
        
    }
}