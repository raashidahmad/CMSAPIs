using CMS.Api.Helpers;
using CMS.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Http;

namespace CMS.Api.Controllers
{
    public class FileController : ApiController
    {
        IDocumentService documentService;
        FilesHelper filesHelper;
        String tempPath = "~/Uploads/";
        String serverMapPath = "~/Uploads/";
        private string StorageRoot
        {
            get { return Path.Combine(HostingEnvironment.MapPath(serverMapPath)); }
        }
        private string urlBase = "Uploads/";
        String deleteURL = "api/File/?file=";
        String deleteType = "DELETE";

        public FileController()
            {
            urlBase = WebConfigurationManager.AppSettings["ApiHostUrl"] + urlBase;
            deleteURL = WebConfigurationManager.AppSettings["ApiHostUrl"] + deleteURL;
            filesHelper = new FilesHelper(deleteURL, deleteType, StorageRoot, urlBase, tempPath, serverMapPath);
            documentService = new DocumentService();
            }

        public string OptionsXXX()
            {
            return null; // HTTP 200 response with empty body
            }

        public IHttpActionResult Get()
            {
            return Ok("Ok");
            }

        public IHttpActionResult Get(int id)
            {
            JsonFiles listOfFiles = filesHelper.GetFileList(id);
            ViewDataUploadFilesResult[] files = listOfFiles.files;
            return Ok(files);
            }

        [HttpPost]
        public IHttpActionResult Post(int id)
            {
            if (id == 0)
                {
                return BadRequest("Invalid Id provided for the complaint");
                }

            var resultList = new List<ViewDataUploadFilesResult>();
            var httpRequest = HttpContext.Current.Request;
            filesHelper.UploadAndShowResults(httpRequest, resultList, id);
            documentService.Add(id, filesHelper.currentFileName);
            JsonFiles files = new JsonFiles(resultList);
            bool isEmpty = !resultList.Any();
            if (isEmpty)
                {
                return Json("Error ");
                }
            else
                {
                return Json(files);
                }
            }

        public IHttpActionResult Delete(int id, string file)
            {
            filesHelper.DeleteFile(id, file);
            documentService.Delete(id, file);
            return Ok(id);
            }
    }
}
