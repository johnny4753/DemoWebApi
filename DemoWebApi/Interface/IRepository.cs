using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DemoWebApi.Interface
{
    public interface IRepository<TEntity> where TEntity : class
    {
        void CloseLazyLoading();
        TEntity Create(TEntity instance);
        void Create(IEnumerable<TEntity> entities);
        void Delete(TEntity instance);
        bool Exist(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TEntity> GetAll();
        void SaveChanges();
        TEntity Update(TEntity instance);
        void Dispose();
    }
}