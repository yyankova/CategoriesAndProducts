using Products.Data;
using Products.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Diagnostics;
using System.Collections.Specialized;
using System.IO;

namespace Products.WebApi.Controllers
{
    public class ProductsController : ApiController
    {
        //TODO: common things in common controller/other class
        //TODO: separate view models for input/output
        private const int PageSize = 5;
        private const string EmptyProductNameMessage = "Product name must not be empty";
        private const string EmptyCategoryMessage = "Product category must not be empty";
        private const string NonExistingCategoryMessage = "Category with id {0} does not exist";
        private const string ProductNotFoundMessage = "Product with id {0} does not exist";

        private IProductsData data;

        public ProductsController()
        {
            this.data = new ProductsData();
        }

        [HttpPost]
        public async Task<IHttpActionResult> Create()
        {
            Product product = new Product();
            return await this.UpdateProduct(product, true);
        }

        [HttpPut]
        [Route("api/products/update/{id}")]
        public async Task<IHttpActionResult> Update(int id)
        {
            var product = this.data
                .Products
                .All()
                .Where(p => p.ProductId == id)
                .FirstOrDefault();

            if (product == null)
            {
                return BadRequest(String.Format(ProductNotFoundMessage, id));
            }

            return await this.UpdateProduct(product, false);
        }

        [HttpGet]
        public IHttpActionResult Get([FromUri]int? page)
        {
            int currentPage = 0;
            if (page != null)
            {
                currentPage = (int)page;
            }

            var products = this.data
                .Products
                .All()
                .Select(p => new ProductModel() { ProductId = p.ProductId, Name = p.Name, Description = p.Description, CategoryName = p.Category.Name })
                .OrderBy(p => p.Name)
                .Skip(currentPage * PageSize)
                .Take(PageSize)
                .ToList();

            return Ok(products);
        }

        [HttpGet]
        [Route("api/products/getbyid/{id}")]
        public IHttpActionResult GetById(int id)
        {
            var product = this.data
                .Products
                .All()
                .Where(p => p.ProductId == id)
                .Select(p => new ProductModel() { ProductId = p.ProductId, Name = p.Name, Description = p.Description, CategoryName = p.Category.Name })
                .FirstOrDefault();

            if (product == null)
            {
                return BadRequest(String.Format(ProductNotFoundMessage, id));
            }

            return Ok(product);
        }

        //TODO: fix, it only works if the two params are present: api/products/search?name=klkl&category=
        [HttpGet]
        public IHttpActionResult Search(int? category, string name)
        {
            var products = this.data
                .Products
                .All();
            if (category != null)
            {
                int categoryId = (int)category;
                products = products.Where(p => p.CategoryId == category);
            }

            if (!String.IsNullOrEmpty(name))
            {
                products = products.Where(p => p.Name.ToLower().Contains(name.ToLower()));
            }

            var result = products
                .Select(p => new ProductModel() { 
                    ProductId = p.ProductId, 
                    Name = p.Name, 
                    Description = p.Description, 
                    CategoryName = p.Category.Name });

            return Ok(result);
        }

        [HttpDelete]
        [Route("api/products/delete/{id}")]
        public IHttpActionResult Delete(int id)
        {
            var product = this.data
                .Products
                .All()
                .Where(p => p.ProductId == id)
                .FirstOrDefault();

            if (product == null)
            {
                return BadRequest(String.Format(ProductNotFoundMessage, id));
            }

            this.data
                .Products
                .Delete(product);
            this.data
                .SaveChanges();

            return Ok();
        }

        private async Task<IHttpActionResult> UpdateProduct(Product dbProduct, bool isNewProduct)
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);

                //Product name
                if (provider.FormData.GetValues("name") == null)
                {
                    return this.BadRequest(EmptyProductNameMessage);
                }

                string name = provider.FormData.GetValues("name")[0];
                if (String.IsNullOrEmpty(name))
                {
                    return this.BadRequest(EmptyProductNameMessage);
                }

                dbProduct.Name = name;

                //Product description
                if (provider.FormData.GetValues("description") != null)
                {
                    dbProduct.Description = provider.FormData.GetValues("description")[0];
                }

                //Product category;
                if (provider.FormData.GetValues("categoryid") == null)
                {
                    return this.BadRequest(EmptyCategoryMessage);
                }

                string categoryIdAsString = provider.FormData.GetValues("categoryid")[0];
                if (String.IsNullOrEmpty(categoryIdAsString))
                {
                    return this.BadRequest(EmptyCategoryMessage);
                }

                int categoryId;
                if (!int.TryParse(categoryIdAsString, out categoryId))
                {
                    return this.BadRequest(String.Format(NonExistingCategoryMessage, categoryId));
                }

                var dbCategory = this.data
                    .Categories
                    .Find(categoryId);
                if (dbCategory == null)
                {
                    return BadRequest(String.Format(NonExistingCategoryMessage, categoryId));
                }

                dbProduct.CategoryId = categoryId;

                //Product image
                if (provider.FileData.Count > 0)
                {
                    MultipartFileData file = provider.FileData[0];
                    if (file != null)
                    {
                        dbProduct.Image = File.ReadAllBytes(file.LocalFileName);
                        File.Delete(file.LocalFileName);
                    }
                }

                if (isNewProduct)
                {
                    this.data
                        .Products
                        .Add(dbProduct);
                }

                this.data
                    .SaveChanges();

                return Ok(dbProduct.ProductId);
            }
            catch (System.Exception e)
            {
                return InternalServerError(e);
            }
        }
    }
}