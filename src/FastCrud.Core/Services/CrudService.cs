using FastCrud.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FastCrud.Abstractions.Abstractions;
using FastCrud.Abstractions.Primitives;
using FastCrud.Abstractions.Query;

namespace FastCrud.Core.Services
{
    /// <summary>
    /// Default implementation of <see cref="ICrudService{TAgg, TId}"/> that orchestrates mapping, validation, querying and persistence.
    /// </summary>
    /// <typeparam name="TAgg">Aggregate type.</typeparam>
    /// <typeparam name="TId">Identifier type.</typeparam>
    public class CrudService<TAgg, TId> : ICrudService<TAgg, TId>
    {
        private readonly IRepository<TAgg, TId> _repository;
        private readonly IObjectMapper _mapper;
        private readonly IEnumerable<IModelValidator<TAgg>> _validators;
        private readonly IQueryEngine _queryEngine;

        /// <summary>
        /// Creates a new <see cref="CrudService{TAgg, TId}"/>.
        /// </summary>
        /// <param name="repository">Repository used for data access.</param>
        /// <param name="mapper">Object mapper used to convert DTOs to aggregates.</param>
        /// <param name="validators">Zero or more validators for the aggregate.</param>
        /// <param name="queryEngine">Engine to apply paging, filtering and sorting.</param>
        public CrudService(IRepository<TAgg, TId> repository, IObjectMapper mapper, IEnumerable<IModelValidator<TAgg>> validators, IQueryEngine queryEngine)
        {
            _repository = repository;
            _mapper = mapper;
            _validators = validators;
            _queryEngine = queryEngine;
        }

        /// <inheritdoc />
        public async Task<TAgg> CreateAsync(object input, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(input);
            // map input to new aggregate instance
            var entity = input is TAgg agg ? agg : _mapper.Map<TAgg>(input);
            
            // validate if any validators are registered
            foreach (var validator in _validators)
            {
                await validator.ValidateAsync(entity, cancellationToken);
            }
            
            await _repository.AddAsync(entity, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return entity;
        }

        /// <inheritdoc />
        public async Task DeleteAsync(TId id, CancellationToken cancellationToken)
        {
            var entity = await _repository.FindAsync(id, cancellationToken);
            if (entity != null)
            {
                await _repository.DeleteAsync(entity, cancellationToken);
                await _repository.SaveChangesAsync(cancellationToken);
            }
        }

        /// <inheritdoc />
        public async Task<TAgg?> GetAsync(TId id, CancellationToken cancellationToken)
        {
            return await _repository.FindAsync(id, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<PagedResult<TAgg>> QueryAsync(IQuerySpec spec, CancellationToken cancellationToken)
        {
            var query = _repository.Query();
            return await _queryEngine.ApplyQueryAsync(query, spec, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<TAgg> UpdateAsync(TId id, object input, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(input);
            var entity = await _repository.FindAsync(id, cancellationToken);
            if (entity == null)
            {
                throw new InvalidOperationException($"{typeof(TAgg).Name} with id {id} not found");
            }
            
            // map input onto existing entity
            if (input is TAgg agg)
            {
                _mapper.Map(agg, entity);
            }
            else
            {
                _mapper.Map(input, entity);
            }
            
            // validate
            foreach (var validator in _validators)
            {
                await validator.ValidateAsync(entity, cancellationToken);
            }
            
            await _repository.SaveChangesAsync(cancellationToken);
            return entity;
        }
    }
}
