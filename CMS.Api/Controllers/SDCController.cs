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
    public class SDCController : ApiController
    {
        private ISDCService sdcService;
        
        public SDCController()
            {

            }

        public IHttpActionResult Get()
            {
            sdcService = new SDCService();
            var sdcs = sdcService.GetAll();
            if (sdcs != null)
                {
                var sdcEntities = sdcs as List<SDCView> ?? sdcs.ToList();
                if (sdcEntities.Any())
                    return Ok(sdcEntities);
                }
            return Content(HttpStatusCode.NotFound, APIMessageHelper.ListNotFoundMessage("SDCs"));
            }

        // GET api/sdc/5
        public IHttpActionResult Get(int id)
            {
            sdcService = new SDCService();
            var sdc = sdcService.GetById(id);
            if (sdc != null)
                return Ok(sdc);

            return Content(HttpStatusCode.NotFound, APIMessageHelper.EntityNotFoundMessage("SDC", id));
            }

        // POST api/sdc
        public IHttpActionResult Post([FromBody] SDC sdc)
            {
            sdcService = new SDCService();
            if (!ModelState.IsValid)
                {
                return BadRequest(ModelState);
                }

            var response = sdcService.Add(sdc);
            if (response.Success)
                {
                return Ok(response.ReturnedId);
                }
            return Content(HttpStatusCode.BadRequest, response.Message);
            }

        // PUT api/sdc/5
        public IHttpActionResult Put(int id, [FromBody]SDC sdc)
            {
            sdcService = new SDCService();
            if (!ModelState.IsValid)
                {
                return BadRequest(ModelState);
                }
            sdcService.Update(id, sdc);
            return StatusCode(HttpStatusCode.OK);
            }

        // DELETE api/sdc/5
        public IHttpActionResult Delete(int id)
            {
            sdcService = new SDCService();
            var response = sdcService.Delete(id);
            if (response.Success)
                return Ok(id);

            return Content(HttpStatusCode.BadRequest, response.Message);
            }
    }
}