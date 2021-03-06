﻿using Accounting.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.DataLayer
{
  [Export(typeof(IRepository<>))]
  public class RepositoryBase<TEntity> : IRepository<TEntity> where TEntity : class
  {
    internal DbContext context;
    
    internal DbSet<TEntity> dbSet;

    [ImportingConstructor]
    public RepositoryBase([Import] DbContext context)
    {
      this.context = context;
      this.dbSet = context.Set<TEntity>();
    }

    public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
      Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
      string includeProperties = "")
    {
      IQueryable<TEntity> query = dbSet;

      if (filter != null)
      {
        query = query.Where(filter);
      }

      foreach (var includeProperty in includeProperties.Split
          (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
      {
        query = query.Include(includeProperty);
      }

      if (orderBy != null)
      {
        return orderBy(query).ToList();
      }
      else
      {
        return query.ToList();
      }
    }

    public virtual TEntity GetByID(object id)
    {
      return dbSet.Find(id);
    }

    public virtual void Create(TEntity entity)
    {
      context.Entry(entity).State = EntityState.Added;
      //dbSet.Add(entity);
    }

    public virtual void Delete(object id)
    {
      TEntity entityToDelete = dbSet.Find(id);
      Delete(entityToDelete);
    }

    public virtual void Delete(TEntity entityToDelete)
    {
      if (context.Entry(entityToDelete).State == EntityState.Detached)
      {
        dbSet.Attach(entityToDelete);
      }
      dbSet.Remove(entityToDelete);
    }

    public virtual void Update(TEntity entityToUpdate)
    {
      dbSet.Attach(entityToUpdate);
      context.Entry(entityToUpdate).State = EntityState.Modified;
    }


    public void Refresh(TEntity revertedTransaction)
    {
      context.Entry(revertedTransaction).Reload();
    }


    public IQueryable<TEntity> Read()
    {
      return context.Set<TEntity>();
    }
  }
}
