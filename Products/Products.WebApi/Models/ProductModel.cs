using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Products.WebApi.Models
{
    public class ProductModel
    {
        public int ProductId { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }

        public int? CategoryId { get; set; }
        
        public string CategoryName { get; set; }
        
        public byte[] Image { get; set; }
    }
}