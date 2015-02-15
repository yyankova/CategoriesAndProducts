using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

using System.Data.Entity;
using System.Data.Entity.Infrastructure;

using Products.Data;
using Products.WebApi.Models;

namespace Products.WebApi.Controllers
{
    public class CategoriesController : ApiController
    {
        //TODO: sorting
        //TODO: StyleCop
        //TODO: Use ninject
        //TODO: Automapper for the view models
        //TODO: Page size as parameter from the client
        //TODO: Unit tests
        private const int PageSize = 5;
        private const string EmptyCategoryNameMessage = "Category name must not be empty";
        private const string CategoryNotFoundMessage = "Category with id {0} does not exist";

        private IProductsData data;

        public CategoriesController()
        {
            this.data = new ProductsData();
        }

        [HttpPost]
        public IHttpActionResult Create(CategoryModel inputCategory)
        {
            if (String.IsNullOrEmpty(inputCategory.Name))
            {
                return this.BadRequest(EmptyCategoryNameMessage);
            }

            Category dbCategory = new Category()
            {
                Name = inputCategory.Name,
                Description = inputCategory.Description
            };
            this.data
                .Categories
                .Add(dbCategory);
            this.data
                .SaveChanges();

            inputCategory.CategoryId = dbCategory.CategoryId;
            return this.Created<CategoryModel>("", inputCategory);
        }

        [HttpGet]
        public IHttpActionResult Get([FromUri]int? page)
        {
            int currentPage = 0;
            if (page != null)
            {
                currentPage = (int)page;
            }

            var categories = this.data
                .Categories
                .All()
                .Select(c => new CategoryModel() { CategoryId = c.CategoryId, Name = c.Name, Description = c.Description })
                .OrderBy(c => c.Name)
                .Skip(currentPage * PageSize)
                .Take(PageSize)
                .ToList();

            return Ok(categories);
        }

        [HttpGet]
        [Route("api/categories/getbyid/{id}")]
        public IHttpActionResult GetById(int id)
        {
            var category = this.data
                .Categories
                .All()
                .Where(c => c.CategoryId == id)
                .Select(c => new CategoryModel() { CategoryId = c.CategoryId, Name = c.Name, Description = c.Description })
                .FirstOrDefault();

            if (category == null)
            {
                return BadRequest(String.Format(CategoryNotFoundMessage, id));
            }

            return Ok(category);
        }

        [HttpPut]
        public IHttpActionResult Update(CategoryModel inputCategory)
        {
            if (String.IsNullOrEmpty(inputCategory.Name))
            {
                return BadRequest(EmptyCategoryNameMessage);
            }

            int id = inputCategory.CategoryId;
            var category = this.data
                .Categories
                .All()
                .Where(c => c.CategoryId == id)
                .FirstOrDefault();

            if (category == null)
            {
                return BadRequest(String.Format(CategoryNotFoundMessage, id));
            }

            category.Name = inputCategory.Name;
            category.Description = inputCategory.Description;
            this.data
                .SaveChanges();

            return Ok(category);
        }

        [HttpDelete]
        [Route("api/categories/delete/{id}")]
        public IHttpActionResult Delete(int id)
        {
            var category = this.data
                .Categories
                .All()
                .Where(c => c.CategoryId == id)
                .FirstOrDefault();

            if (category == null)
            {
                return BadRequest(String.Format(CategoryNotFoundMessage, id));
            }

            if (category.Products.Count > 0)
            {
                foreach (var product in category.Products)
                {
                    this.data
                        .Products
                        .Delete(product);
                }
            }

            this.data
                .Categories
                .Delete(category);
            this.data
                .SaveChanges();

            return Ok();
        }
    }
}