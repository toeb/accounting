using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Accounting.Model
{
  public interface IRepository<TEntity> where TEntity : class
  {
    IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
      Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
      string includeProperties = "");


    IQueryable<TEntity> Read();


    TEntity GetByID(object id);

    /// <summary>
    /// creates the specified entity in the database. 
    /// 
    /// </summary>
    /// <param name="entity"></param>
    void Create(TEntity entity);

    void Delete(object id);

    void Delete(TEntity entityToDelete);

    void Update(TEntity entityToUpdate);

    void Refresh(TEntity revertedTransaction);
  }

}