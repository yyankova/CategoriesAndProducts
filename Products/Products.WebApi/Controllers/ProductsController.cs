﻿using Products.Data;
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
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);

                Product dbProduct = new Product();
                foreach (var key in provider.FormData.AllKeys)
                {
                    string val = provider.FormData.GetValues(key)[0];
                    string property = key.ToLower();
                    if (property == "name")
                    {
                        if (String.IsNullOrEmpty(val))
                        {
                            return this.BadRequest(EmptyProductNameMessage);
                        }

                        dbProduct.Name = val;
                    }
                    else if (property == "description")
                    {
                        dbProduct.Description = val;
                    }
                    else if (property == "categoryid")
                    {
                        if (String.IsNullOrEmpty(val))
                        {
                            return this.BadRequest(EmptyCategoryMessage);
                        }

                        int categoryId = int.Parse(val);
                        var dbCategory = this.data
                            .Categories
                            .Find(categoryId);

                        if (dbCategory == null)
                        {
                            return BadRequest(String.Format(NonExistingCategoryMessage, categoryId));
                        }

                        dbProduct.CategoryId = categoryId;
                    }
                }

                MultipartFileData file = provider.FileData[0];
                if (file != null)
                {
                    dbProduct.Image = File.ReadAllBytes(file.LocalFileName);
                }

                this.data
                    .Products
                    .Add(dbProduct);
                this.data
                    .SaveChanges();

                //TODO: check location
                return this.Created<int>("", dbProduct.ProductId);
            }
            catch (System.Exception e)
            {
                return InternalServerError(e);
            }
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

        //[HttpGet]
        //public IHttpActionResult Search(int? category, string name)
        //{
        //    var products = this.data
        //        .Products
        //        .All();
        //    if (category != null)
        //    {
        //        int categoryId = (int)category;
        //        products.Where(p => p.CategoryId == category);
        //    }

        //    if (!String.IsNullOrEmpty(name))
        //    {
        //        products.Where(p => p.Name.ToLower().Contains(name.ToLower()));
        //    }

        //    products
        //        .Select(p => new ProductModel() { ProductId = p.ProductId, Name = p.Name, Description = p.Description, CategoryName = p.Category.Name });

        //    return Ok(products);
        //}

        //[HttpPut]
        //public IHttpActionResult Update(ProductModel inputProduct)
        //{
        //    if (String.IsNullOrEmpty(inputProduct.Name))
        //    {
        //        return BadRequest(EmptyProductNameMessage);
        //    }

        //    int id = inputProduct.ProductId;
        //    var product = this.data
        //        .Products
        //        .All()
        //        .Where(p => p.ProductId == id)
        //        .FirstOrDefault();

        //    if (product == null)
        //    {
        //        return BadRequest(String.Format(ProductNotFoundMessage, id));
        //    }

        //    product.Name = inputProduct.Name;
        //    product.Description = inputProduct.Description;
        //    if (inputProduct.CategoryId != null)
        //    {
        //        int categoryId = (int)inputProduct.CategoryId;
        //        var dbCategory = this.data
        //            .Categories
        //            .Find(categoryId);
        //        if (dbCategory != null)
        //        {
        //            product.CategoryId = categoryId;
        //        } 
        //    }

        //    this.data
        //        .SaveChanges();

        //    return Ok(product);
        //}

        //[HttpDelete]
        //[Route("api/products/delete/{id}")]
        //public IHttpActionResult Delete(int id)
        //{
        //    var product = this.data
        //        .Products
        //        .All()
        //        .Where(p => p.ProductId == id)
        //        .FirstOrDefault();

        //    if (product == null)
        //    {
        //        return BadRequest(String.Format(ProductNotFoundMessage, id));
        //    }

        //    this.data
        //        .Products
        //        .Delete(product);
        //    this.data
        //        .SaveChanges();

        //    return Ok();
        //}
    }
}