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
    public class ComplaintController : ApiController
        {
        private IComplaintService complaintService;
        /// <summary>
        /// Public constructor to initialize Complaint service instance
        /// </summary>
        public ComplaintController()
            {
            }

        public IHttpActionResult Get()
            {
            complaintService = new ComplaintService();
            var complaints = complaintService.GetAll();
            if (complaints != null)
                {
                var complaintEntities = complaints as List<ComplaintView> ?? complaints.ToList();
                if (complaintEntities.Any())
                    return Ok(complaintEntities);
                }
            return Content(HttpStatusCode.NotFound, APIMessageHelper.ListNotFoundMessage("Complaints"));
            }

        // GET api/Complaint/5
        public IHttpActionResult Get(int id)
            {
            complaintService = new ComplaintService();
            var Complaint = complaintService.GetById(id);
            if (Complaint != null)
                return Ok(Complaint);

            return Content(HttpStatusCode.NotFound, APIMessageHelper.EntityNotFoundMessage("Complaint", id));
            }

        // POST api/Complaint
        public IHttpActionResult Post([FromBody] NewComplaint complaint)
            {
            List<string> documents = new List<string>();
            complaintService = new ComplaintService();
            if (!ModelState.IsValid)
                {
                return BadRequest(ModelState);
                }

            var response = complaintService.Add(complaint, documents);
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

        // PUT api/Complaint/5
        //Need to modify it as what is actually allowed to update
        public IHttpActionResult Put(int id, [FromBody]NewComplaint complaint)
            {
            complaintService = new ComplaintService();
            if (!ModelState.IsValid)
                {
                return BadRequest(ModelState);
                }
            complaintService.Update(id, complaint);
            return StatusCode(HttpStatusCode.OK);
            }

        // DELETE api/Complaint/5
        public IHttpActionResult Delete(int id)
            {
            complaintService = new ComplaintService();
            var response = complaintService.Delete(id);
            if (response.Success)
                return Ok(id);

            return Content(HttpStatusCode.BadRequest, response.Message);
            }

        }
}