using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Data
{
    public interface IProductsData
    {
        IRepository<Category> Categories { get; }

        IRepository<Product> Products { get; }

        int SaveChanges();
    }
}
