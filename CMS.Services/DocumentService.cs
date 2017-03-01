using CMS.DataModel.Models;
using CMS.DataModel.ModelWrapper;
using CMS.DataModel.UnitOfWork;
using CMS.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Services
    {
    
    public interface IDocumentService
        {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        ActionResponse Add(int complaintId, string fileName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="complaintId"></param>
        /// <returns></returns>
        ActionResponse Delete(int complaintId, string fileName);
        }
    
    public class DocumentService : IDocumentService
        {
        ActionResponse response;
        IMessageHelper messageHelper;
        UnitOfWork unitWork;

        public DocumentService()
            {
            response = new ActionResponse();
            response.Success = true;
            response.Message = "Ok";
            messageHelper = new MessageHelper();
            unitWork = new UnitOfWork();
            }

        public ActionResponse Add(int complaintId, string fileName)
            {
            if (complaintId == 0)
                {
                response.Success = false;
                response.Message = messageHelper.GetRequiredMessage("Valid Complaint Id");
                return response;
                }

            if (string.IsNullOrEmpty(fileName))
                {
                response.Success = false;
                response.Message = messageHelper.GetRequiredMessage("Valid File Name");
                return response;
                }

            var complaint = unitWork.ComplaintRepository.GetByID(complaintId);
            if (complaint == null)
                {
                response.Success = false;
                response.Message = messageHelper.GetNotFoundMessage("Complaint");
                return response;
                }

            var newEntry = unitWork.DocumentsRepository.Insert(new EFDocuments()
                {
                    Complaint = complaint,
                    Path = fileName
                });

            unitWork.Save();
            response.ReturnedId = newEntry.Id;
            return response;
            }

        public ActionResponse Delete(int complaintId, string fileName)
            {
            if (complaintId == 0)
                {
                response.Success = false;
                response.Message = messageHelper.GetInvalidNumericMessaage("Complaint Id");
                return response;
                }

            if (string.IsNullOrEmpty(fileName))
                {
                response.Success = false;
                response.Message = messageHelper.GetInvalidMessage("File Name");
                return response;
                }

            var complaint = unitWork.ComplaintRepository.GetByID(complaintId);
            if (complaint == null)
                {
                response.Success = false;
                response.Message = messageHelper.GetNotFoundMessage("Complaint");
                return response;
                }

            var document = unitWork.DocumentsRepository.GetWithInclude(d => d.Complaint.Id == complaintId && d.Path == fileName, new string[] { "Complaint" }).FirstOrDefault();
            if (document == null)
                {
                response.Success = false;
                response.Message = messageHelper.GetNotFoundMessage("File Name");
                return response;
                }

            unitWork.DocumentsRepository.Delete(document);
            unitWork.Save();
            return response;
            }
        }
    }
