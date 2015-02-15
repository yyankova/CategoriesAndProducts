using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Products.Data
{
    public class ProductsData : IProductsData
    {
        private DbContext context;
        private IDictionary<Type, object> repositories;

        public ProductsData() : this(new ProductsDBEntities())
        {
        }

        public ProductsData(DbContext context)
        {
            this.context = context;
            this.repositories = new Dictionary<Type, object>();
        }

        public IRepository<Category> Categories
        {
            get
            {
                return this.GetRepository<Category>();
            }
        }

        public IRepository<Product> Products
        {
            get
            {
                return this.GetRepository<Product>();
            }
        }

        public int SaveChanges()
        {
            return this.context.SaveChanges();
        }


        private IRepository<T> GetRepository<T>() where T : class
        {
            var typeOfRepository = typeof(T);
            if (!this.repositories.ContainsKey(typeOfRepository))
            {
                var newRepository = Activator.CreateInstance(typeof(EFRepository<T>), context);
                this.repositories.Add(typeOfRepository, newRepository);
            }

            return (IRepository<T>)this.repositories[typeOfRepository];
        }
    }
}
