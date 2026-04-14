using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Lab4_5.Data
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly AppDbContext Context;
        private readonly DbSet<TEntity> _entities;

        public Repository(AppDbContext context)
        {
            Context = context;
            _entities = context.Set<TEntity>();
        }

        public TEntity Get(int id) => _entities.Find(id);

        public IEnumerable<TEntity> GetAll() => _entities.ToList();

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
            => _entities.Where(predicate);

        public void Add(TEntity entity) => _entities.Add(entity);

        public void AddRange(IEnumerable<TEntity> entities) => _entities.AddRange(entities);

        public void Remove(TEntity entity) => _entities.Remove(entity);

        public void RemoveRange(IEnumerable<TEntity> entities) => _entities.RemoveRange(entities);
    }
}
