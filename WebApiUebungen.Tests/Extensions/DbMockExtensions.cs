using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.Language.Flow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApiUebungen.Tests.Utils;

namespace Moq
{
    public static class DbMockExtensions
    {
        public static void SetupDbSet<T>(this Mock<DbSet<T>> mock, List<T> list, CancellationToken cancellationToken = default)
            where T : class
        {
            var copy = list.ToList();

            mock.As<IAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<T>(copy.GetEnumerator()));

            mock.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<T>(copy.AsQueryable().Provider));

            mock.As<IQueryable<T>>()
                .Setup(m => m.Expression)
                .Returns(copy.AsQueryable().Expression);

            mock.As<IQueryable<T>>()
                .Setup(m => m.ElementType)
                .Returns(copy.AsQueryable().ElementType);

            mock.As<IQueryable<T>>()
                .Setup(m => m.GetEnumerator())
                .Returns(copy.AsQueryable().GetEnumerator());

            // m => m.ToList() ist nicht unterstützt, da ToList eine **Extension-Methode** ist vom DbSet
            //mock.As<IQueryable<T>>()
            //    .Setup(m => m.ToList())
            //    .Returns(copy.ToList());

            // Setup Add

            mock.Setup(d => d.Add(It.IsAny<T>()))
                .Callback<T>((s) => copy.Add(s));

            // Setup Update

            mock.Setup(d => d.Update(It.IsAny<T>()));

            // Remove

            mock.Setup(d => d.Remove(It.IsAny<T>()))
                .Callback<T>((s) => copy.Remove(s));
        }


        public static ISetupGetter<TContext, DbSet<TEntity>> SetupCrud<TContext, TEntity>(this Mock<TContext> dbMock, Expression<Func<TContext, DbSet<TEntity>>> expression, List<TEntity> source, CancellationToken cancellationToken = default)
            where TContext : DbContext
            where TEntity : class
        {
            dbMock.Setup(db => db.Add(It.IsAny<TEntity>()))
                .Callback<object>(item => source.Add((TEntity)item));

            dbMock.Setup(db => db.AddAsync(It.IsAny<object>(), cancellationToken))
                .Callback<object, CancellationToken>((item, token) => source.Add((TEntity)item));

            dbMock.Setup(db => db.AddAsync(It.IsAny<TEntity>(), cancellationToken))
                .Callback<object, CancellationToken>((item, token) => source.Add((TEntity)item));

            dbMock.Setup(db => db.Remove(It.IsAny<object>()))
                .Callback<object>((item) => source.Remove((TEntity)item));

            dbMock.Setup(db => db.Remove(It.IsAny<TEntity>()))
                .Callback<object>((item) => source.Remove((TEntity)item));

            dbMock.Setup(db => db.RemoveRange(It.IsAny<IEnumerable<object>>()))
                .Callback<IEnumerable<object>>((items) =>
                    items.Cast<TEntity>().ToList().ForEach(item => source.Remove(item)));

            return dbMock.SetupGet(expression);
        }

        public static Mock<DbSet<TEntity>> SetupDbSetMock<TContext, TEntity>(this Mock<TContext> dbMock, Expression<Func<TContext, DbSet<TEntity>>> expression, List<TEntity> source, CancellationToken cancellationToken = default)
            where TContext : DbContext
            where TEntity : class
        {
            var dbSetMock = new Mock<DbSet<TEntity>>();
            dbSetMock.SetupDbSet(source, cancellationToken);

            dbMock.SetupCrud(expression, source, cancellationToken).Returns(dbSetMock.Object);

            return dbSetMock;
        }
    }
}
