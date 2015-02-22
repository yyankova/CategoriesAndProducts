using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Products.WebApi.Models
{
    public class ProductSummaryModel
    {
        public int ProductId { get; set; }

        public string Name { get; set; }

        public bool HasImage { get; set; }
    }
}