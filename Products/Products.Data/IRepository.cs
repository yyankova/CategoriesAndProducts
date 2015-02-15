using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Data
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> All();

        T Find(object id);

        void Add(T entity);

        void Update(T entity);

        T Delete(T entity);

        T Delete(object id);

        int SaveChanges();
    }
}
