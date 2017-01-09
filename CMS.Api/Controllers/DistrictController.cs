using CMS.Api.Helpers;
using CMS.DataModel.ModelWrapper;
using CMS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace CMS.Api.Controllers
{
    public class DistrictController : ApiController
    {
        private IDistrictService districtService;
        /// <summary>
        /// Public constructor to initialize district service instance
        /// </summary>
        public DistrictController()
            {
            //districtService = new DistrictService();
            }
        
        public IHttpActionResult Get()
            {
            districtService = new DistrictService();
            var categories = districtService.GetAll();
            if (categories != null)
            {
                var districtEntities = categories as List<District> ?? categories.ToList();
                if (districtEntities.Any())
                    return Ok(districtEntities);
            }
            return Content(HttpStatusCode.NotFound, APIMessageHelper.ListNotFoundMessage("Districts"));
            }

        // GET api/district/5
        public IHttpActionResult Get(int id)
            {
            districtService = new DistrictService();
            var district = districtService.GetById(id);
            if (district != null)
                return Ok(district);
            
            return Content(HttpStatusCode.NotFound, APIMessageHelper.EntityNotFoundMessage("District", id));
            }

        // POST api/district
        public IHttpActionResult Post([FromBody] District district)
            {
            districtService = new DistrictService();
            if (!ModelState.IsValid)
                {
                return BadRequest(ModelState);
                }

            var response = districtService.Add(district);
            if(response.Success)
                {
                return Ok(response.ReturnedId);
                }
            return Content(HttpStatusCode.BadRequest, response.Message);
            }

        // PUT api/district/5
        public IHttpActionResult Put(int id, [FromBody]District district)
            {
            districtService = new DistrictService();
            if (!ModelState.IsValid)
                {
                return BadRequest(ModelState);
                }
            districtService.Update(id, district);
            return StatusCode(HttpStatusCode.OK);
            }

        // DELETE api/district/5
        public IHttpActionResult Delete(int id)
            {
            districtService = new DistrictService();
            var response = districtService.Delete(id);
            if (response.Success)
                return Ok(id);
            
            return Content(HttpStatusCode.BadRequest, response.Message);
            }
    }
}