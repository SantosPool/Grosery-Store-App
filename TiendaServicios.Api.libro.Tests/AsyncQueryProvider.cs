using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TiendaServicios.Api.libro.Tests
{
    public class AsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider inner;
        public AsyncQueryProvider(IQueryProvider _inner)
        {
            inner = _inner;
        }
        public IQueryable CreateQuery(Expression expression)
        {
            return new AsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new AsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return inner.Execute<TResult>(expression);
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            var resultadoType = typeof(TResult).GetGenericArguments()[0];
            var executeResult = typeof(IQueryProvider)
                                .GetMethod(
                                   name: nameof(IQueryProvider.Execute),
                                   genericParameterCount: 1,
                                   types: new[] { typeof(Expression) }
                                   )
                                .MakeGenericMethod(resultadoType)
                                .Invoke(this, new[] { expression });

            return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))?
                .MakeGenericMethod(resultadoType).Invoke(null, new[] { executeResult });
        }
    }
}
