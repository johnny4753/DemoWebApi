using DemoWebApi.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace DemoWebApi.Repository
{
    public class Repository<TEntity> where TEntity : class
    {
        public Repository()
                    : this(new NorthwindDb())
        {
        }

        public Repository(DbContext context)
        {
            Context = context;
            CloseLazyLoading();
        }

        protected DbContext Context { get; set; }

        public void CloseLazyLoading()
        {
            Context.Configuration.LazyLoadingEnabled = false;
        }

        public TEntity Create(TEntity instance)
        {
            var entity = Context.Set<TEntity>().Add(instance);
            SaveChanges();
            return entity;
        }

        public void Create(IEnumerable<TEntity> entities)
        {
            Context.Set<TEntity>().AddRange(entities);
            SaveChanges();
        }

        public void Delete(TEntity instance)
        {
            Context.Entry(instance).State = EntityState.Deleted;
            SaveChanges();
        }

        public bool Exist(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Any(predicate);
        }

        public TEntity Get(Expression<Func<TEntity, bool>> predicate)
        {
            TEntity entity = Context.Set<TEntity>().FirstOrDefault(predicate);
            return entity;
        }

        public IQueryable<TEntity> GetAll()
        {
            return Context.Set<TEntity>().AsQueryable();
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }

        public TEntity Update(TEntity instance)
        {
            Context.Entry(instance).State = EntityState.Modified;
            SaveChanges();
            return instance;
        }

        #region -- dispose context --

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (Context == null) return;
            Context.Dispose();
            Context = null;
        }

        #endregion -- dispose context --
    }
}