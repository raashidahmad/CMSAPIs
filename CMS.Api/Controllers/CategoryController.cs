using CMS.Api.Helpers;
using CMS.DataModel.ModelWrapper;
using CMS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace CMS.Api.Controllers
{
    public class CategoryController : ApiController
        {
        private ICategoryService categoryService;
        /// <summary>
        /// Public constructor to initialize Category service instance
        /// </summary>
        public CategoryController()
            {
            }

        public IHttpActionResult Get()
            {
            categoryService = new CategoryService();
            var categories = categoryService.GetAll();
            if (categories != null)
                {
                var categoryEntities = categories as List<Category> ?? categories.ToList();
                if (categoryEntities.Any())
                    return Ok(categoryEntities);
                }
            return Content(HttpStatusCode.NotFound, APIMessageHelper.ListNotFoundMessage("Categories"));
            }

        // GET api/Category/5
        public IHttpActionResult Get(int id)
            {
            categoryService = new CategoryService();
            var Category = categoryService.GetById(id);
            if (Category != null)
                return Ok(Category);

            return Content(HttpStatusCode.NotFound, APIMessageHelper.EntityNotFoundMessage("Category", id));
            }

        // POST api/Category
        public IHttpActionResult Post([FromBody] Category category)
            {
            categoryService = new CategoryService();
            if (!ModelState.IsValid)
                {
                return BadRequest(ModelState);
                }

            var response = categoryService.Add(category);
            if (response.Success)
                {
                return Ok(response.ReturnedId);
                }
            return Content(HttpStatusCode.BadRequest, response.Message);
            }

        public IHttpActionResult PostDocument()
            {
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
                {
                var docfiles = new List<string>();
                foreach (string file in httpRequest.Files)
                    {
                    var postedFile = httpRequest.Files[file];
                    var filePath = HttpContext.Current.Server.MapPath("~/" + postedFile.FileName);
                    postedFile.SaveAs(filePath);

                    docfiles.Add(filePath);
                    }
                return Content(HttpStatusCode.Created, docfiles);
                }

            return Content(HttpStatusCode.BadRequest, "");
            }

        // PUT api/Category/5
        public IHttpActionResult Put(int id, [FromBody]Category category)
            {
            categoryService = new CategoryService();
            if (!ModelState.IsValid)
                {
                return BadRequest(ModelState);
                }
            categoryService.Update(id, category);
            return StatusCode(HttpStatusCode.OK);
            }

        // DELETE api/Category/5
        public IHttpActionResult Delete(int id)
            {
            categoryService = new CategoryService();
            var response = categoryService.Delete(id);
            if (response.Success)
                return Ok(id);

            return Content(HttpStatusCode.BadRequest, response.Message);
            }
    }
}