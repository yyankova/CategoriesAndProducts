﻿using System;
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
using System.Web.Http.Cors;

using Products.Data;
using Products.WebApi.Models;
using System.Net.Http.Headers;

namespace Products.WebApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ProductsController : ApiController
    {
        private const int PageSize = 5;
        private const string EmptyProductNameMessage = "Product name must not be empty";
        private const string EmptyCategoryMessage = "Product category must not be empty";
        private const string NonExistingCategoryMessage = "Category with id {0} does not exist";
        private const string ProductNotFoundMessage = "Product with id {0} does not exist";

        private IProductsData data;

        public ProductsController()
            : this(new ProductsData())
        {
        }

        public ProductsController(IProductsData data)
        {
            this.data = data;
        }

        //TODO: Fix pic is accessed at same time
        [HttpGet]
        [Route("api/products/img/{id}")]
        public HttpResponseMessage Image(int id)
        {
            var image = this.data
                .Products
                .All()
                .Where(p => p.ProductId == id)
                .Select(p => new { p.Image, p.ImageExtension })
                .FirstOrDefault();

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            if (image.Image == null)
            {
                string path = HttpRuntime.AppDomainAppPath + "App_Data/canada.jpg";
                var stream = new FileStream(path, FileMode.Open);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                return result;
            }

            result.Content = new ByteArrayContent(image.Image);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/" + image.ImageExtension);
            return result;
        }

        [HttpGet]
        [Route("api/products/byid/{id}")]
        public IHttpActionResult GetById(int id)
        {
            var product = this.data
                .Products
                .All()
                .Where(p => p.ProductId == id)
                .Select(p => new ProductModel() { 
                    ProductId = p.ProductId, 
                    Name = p.Name, 
                    Description = p.Description, 
                    CategoryName = p.Category.Name, 
                    CategoryId = p.CategoryId 
                })
                .FirstOrDefault();

            if (product == null)
            {
                return BadRequest(String.Format(ProductNotFoundMessage, id));
            }

            return Ok(product);
        }

        [HttpGet]
        public IHttpActionResult Search(int? category, string name, int page)
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
                .Select(p => new ProductSummaryModel()
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    HasImage = p.Image == null ? false : true
                })
                .OrderBy(p => p.Name)
                .Skip(page * PageSize)
                .Take(PageSize);

            return Ok(result);
        }

        [HttpDelete]
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

        [HttpPost]
        public async Task<IHttpActionResult> UpdateProduct()
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
                //Validate product name
                if (provider.FormData.GetValues("name") == null)
                {
                    return this.BadRequest(EmptyProductNameMessage);
                }

                string name = provider.FormData.GetValues("name")[0];
                if (String.IsNullOrEmpty(name))
                {
                    return this.BadRequest(EmptyProductNameMessage);
                }

                //Validate product category;
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

                //Validate http method
                var httpMethodInForm = provider.FormData.GetValues("method");
                if (httpMethodInForm == null)
                {
                    return BadRequest();
                }
                
                string httpMethod = httpMethodInForm[0];
                if (httpMethod != "put" && httpMethod != "post")
                {
                    return BadRequest();
                }

                //Create or find by id the product
                Product dbProduct;
                if (httpMethod == "put")
                {
                    var productIdInForm = provider.FormData.GetValues("id");
                    if (productIdInForm == null)
                    {
                        return BadRequest();
                    }

                    int id;
                    if (!int.TryParse(productIdInForm[0], out id))
                    {
                        return BadRequest();
                    }

                    dbProduct = this.data
                                .Products
                                .All()
                                .Where(p => p.ProductId == id)
                                .FirstOrDefault();

                    if (dbProduct == null)
                    {
                        return BadRequest(String.Format(ProductNotFoundMessage, id));
                    }
                }
                else
                {
                    dbProduct = new Product();
                }

                //Set product fields
                dbProduct.Name = name;
                if (provider.FormData.GetValues("description") != null)
                {
                    dbProduct.Description = provider.FormData.GetValues("description")[0];
                }

                dbProduct.CategoryId = categoryId;

                //Set product image
                if (provider.FileData.Count > 0)
                {
                    MultipartFileData file = provider.FileData[0];
                    if (file != null)
                    {
                        dbProduct.Image = File.ReadAllBytes(file.LocalFileName);
                        string fileName = file.Headers.ContentDisposition.FileName.Trim('"');
                        dbProduct.ImageExtension = Path.GetExtension(fileName).Trim('.');
                        File.Delete(file.LocalFileName);
                    }
                }

                if (httpMethod == "post")
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