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
    public class ComplaintController : ApiController
        {
        private IComplaintService complaintService;
        /// <summary>
        /// Public constructor to initialize Complaint service instance
        /// </summary>
        public ComplaintController()
            {
            }

        public string OptionsXXX()
            {
            return null; // HTTP 200 response with empty body
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

        public IHttpActionResult Post([FromBody] NewComplaint complaint)
            {
            List<string> documents = new List<string>();
            complaintService = new ComplaintService();
            if (!ModelState.IsValid)
                {
                return BadRequest(ModelState);
                }

            var response = complaintService.Add(complaint);
            if (response.Success)
                {
                //Create a new directory for the complaint created just now
                string directoryName = "Complaints-" + response.ReturnedId;
                var mappedPath = HttpContext.Current.Server.MapPath("~/Uploads/");
                System.IO.Directory.CreateDirectory(mappedPath + directoryName);
                return Ok(response.ReturnedId);
                }
            return Content(HttpStatusCode.BadRequest, response.Message);
            }

        [Route("api/Complaint/Upload")]
        public IHttpActionResult UploadDocument(int id)
            {
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
                {
                var docfiles = new List<string>();
                foreach (string file in httpRequest.Files)
                    {
                    var postedFile = httpRequest.Files[file];
                    var filePath = HttpContext.Current.Server.MapPath("~/Uploads/" + postedFile.FileName);
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