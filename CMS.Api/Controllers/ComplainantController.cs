using CMS.Api.Helpers;
using CMS.DataModel.ModelWrapper;
using CMS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;

namespace CMS.Api.Controllers
{
    [RoutePrefix("api/complainant")]
    public class ComplainantController : ApiController
    {
        private IComplainantService complainantService;
        /// <summary>
        /// Public constructor to initialize Complainant service instance
        /// </summary>
        public ComplainantController()
            {
            }

        public IHttpActionResult Get()
            {
            complainantService = new ComplainantService();
            var complainants = complainantService.GetAll();
            if (complainants != null)
                {
                var complainantEntities = complainants as List<Complainant> ?? complainants.ToList();
                if (complainantEntities.Any())
                    return Ok(complainantEntities);
                }
            return Content(HttpStatusCode.NotFound, APIMessageHelper.ListNotFoundMessage("Complainants"));
            }

        [Route("GetShortList")]
        public IHttpActionResult GetShortList()
            {
            complainantService = new ComplainantService();
            var complainants = complainantService.GetAll();
            List<ComplainantList> complainantList = new List<ComplainantList>();
            foreach(var complainant in complainants)
                {
                complainantList.Add(new ComplainantList()
                    {
                        Id = complainant.Id,
                        NIC = complainant.NIC,
                        FullName = complainant.FullName
                    });
                }
            if (complainantList.Count > 0)
                return Ok(complainantList);
            else
                return Content(HttpStatusCode.NotFound, APIMessageHelper.ListNotFoundMessage("Complainants"));
            }

        // GET api/Complainant/5
        public IHttpActionResult Get(int id)
            {
            complainantService = new ComplainantService();
            var Complainant = complainantService.GetById(id);
            if (Complainant != null)
                return Ok(Complainant);

            return Content(HttpStatusCode.NotFound, APIMessageHelper.EntityNotFoundMessage("Complainant", id));
            }

        // POST api/Complainant
        public IHttpActionResult Post([FromBody] Complainant complainant)
            {
            complainantService = new ComplainantService();
            if (!ModelState.IsValid)
                {
                return BadRequest(ModelState);
                }

            var response = complainantService.Add(complainant);
            if (response.Success)
                {
                return Ok(response.ReturnedId);
                }
            return Content(HttpStatusCode.BadRequest, response.Message);
            }

        // PUT api/Complainant/5
        public IHttpActionResult Put(int id, [FromBody]Complainant complainant)
            {
            complainantService = new ComplainantService();
            if (!ModelState.IsValid)
                {
                return BadRequest(ModelState);
                }
            complainantService.Update(id, complainant);
            return StatusCode(HttpStatusCode.OK);
            }

        // DELETE api/Complainant/5
        public IHttpActionResult Delete(int id)
            {
            complainantService = new ComplainantService();
            var response = complainantService.Delete(id);
            if (response.Success)
                return Ok(id);

            return Content(HttpStatusCode.BadRequest, response.Message);
            }
    }
}