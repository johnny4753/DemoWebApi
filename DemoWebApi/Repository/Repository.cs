using DemoWebApi.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using DemoWebApi.Interface;

namespace DemoWebApi.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
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