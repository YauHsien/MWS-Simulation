using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mws_sim
{
    public interface IRepository<T> where T : IDatabaseTable
    {
        T GetById(Guid? id);
        IEnumerable<T> List();
        IEnumerable<T> List(Func<T, bool> predicate);
        void Add(T entity);
        void Add(IEnumerable<T> entities);
        void Edit(T entity);
        void Edit(IEnumerable<T> entities);
        void Delete(T entity);
        void Delete(IEnumerable<T> entities);
    }
}
