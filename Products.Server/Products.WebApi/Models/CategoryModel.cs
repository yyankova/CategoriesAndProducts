using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Products.WebApi.Models
{
    public class CategoryModel
    {
        public int CategoryId { get; set; }
        
        public string Name { get; set; }

        public string Description { get; set; }
    }
}