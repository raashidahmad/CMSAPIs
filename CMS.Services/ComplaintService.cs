using AutoMapper;
using CMS.DataModel.Models;
using CMS.DataModel.ModelWrapper;
using CMS.DataModel.UnitOfWork;
using CMS.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace CMS.Services
    {
    
    public interface IComplaintService
        {
        /// <summary>
        /// Gets list of all complaints
        /// </summary>
        /// <returns></returns>
        IEnumerable<ComplaintView> GetAll();

        /// <summary>
        /// Gets a Complaint by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ComplaintView GetById(int id);

        /// <summary>
        /// Adds a new complaint along with uploaded documents
        /// </summary>
        /// <param name="newCompliant"></param>
        /// <param name="documents"></param>
        /// <returns></returns>
        ActionResponse Add(NewComplaint newCompliant);

        /// <summary>
        /// Updates the provided Complaint
        /// </summary>
        /// <param name="ComplaintId"></param>
        /// <param name="updatedComplaint"></param>
        /// <returns></returns>
        ActionResponse Update(int complaintId, NewComplaint updatedComplaint);

        /// <summary>
        /// Deletes the provided Complaint
        /// </summary>
        /// <param name="ComplaintId"></param>
        /// <returns></returns>
        ActionResponse Delete(int complaintId);
        }

    public class ComplaintService : IComplaintService
        {
        private readonly UnitOfWork unitWork;
        IMessageHelper msgHelper;
        ActionResponse response;

        public ComplaintService()
            {
            unitWork = new UnitOfWork();
            msgHelper = new MessageHelper();
            response = new ActionResponse();
            /*config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EFComplaint, ComplaintView>();
            });*/
            }

        public IEnumerable<ComplaintView> GetAll()
            {
            using (var unitWork = new UnitOfWork())
                {
                List<ComplaintView> complaintsList = new List<ComplaintView>();
                var complaints = unitWork.ComplaintRepository.GetWithInclude(c => c.Id != 0, new string[] { "Complainant", "District", "SDC", "Category", "Documents" });
                foreach(var complaint in complaints)
                    {
                    List<string> documents = new List<string>();
                    foreach(var document in complaint.Documents)
                        {
                        documents.Add(document.Path);
                        }

                    complaintsList.Add(new ComplaintView()
                        {
                            Id = complaint.Id,
                            Description = complaint.Description,
                            ComplainantId = complaint.Complainant.Id,
                            Complainant = complaint.Complainant.FullName,
                            SDCId = complaint.SDC.Id,
                            SDC = complaint.SDC.Title,
                            CategoryId = complaint.Category.Id,
                            Category = complaint.Category.Name,
                            District = complaint.District.Name,
                            DistrictId = complaint.District.Id,
                            Documents = documents,
                            Dated = complaint.Dated
                        });
                    }
                return complaintsList;
                }
            }

        public ComplaintView GetById(int id)
            {
            var complaint = unitWork.ComplaintRepository.GetByID(id);
            if (complaint != null)
                {

                }
            return null;
            }

        public ActionResponse Add(NewComplaint newComplaint)
            {
            try
                {
                var category = unitWork.CategoryRepository.GetByID(newComplaint.CategoryId);
                if (category == null)
                    {
                    response.Message = msgHelper.GetNotFoundMessage("Category");
                    response.Success = false;
                    return response;
                    }

                var district = unitWork.DistrictRepository.GetByID(newComplaint.DistrictId);
                if (district == null)
                    {
                    response.Message = msgHelper.GetNotFoundMessage("District");
                    response.Success = false;
                    return response;
                    }

                var sdc = unitWork.SDCRepository.GetByID(newComplaint.SDCId);
                if (sdc == null)
                    {
                    response.Message = msgHelper.GetNotFoundMessage("SDC");
                    response.Success = false;
                    return response;
                    }

                var complainant = unitWork.ComplainantRepository.GetByID(newComplaint.ComplainantId);
                if (complainant == null)
                    {
                    response.Message = msgHelper.GetNotFoundMessage("Complainant");
                    response.Success = false;
                    return response;
                    }

                using (var scope = new TransactionScope())
                    {
                    var complaint = new EFComplaint()
                    {
                        Category = category,
                        Complainant = complainant,
                        District = district,
                        SDC = sdc,
                        Description = newComplaint.Description,
                        Dated = DateTime.Now,
                        Status = EFComplaint.ComplaintStatus.InQueue
                    };

                    unitWork.ComplaintRepository.Insert(complaint);
                    //Store references to all the uploaded documents against a complaint
                    /*if (documents.Count > 0)
                        {
                        List<EFDocuments> docs = new List<EFDocuments>();
                        foreach(var docPath in documents)
                            {
                            docs.Add(new EFDocuments()
                            {
                                Complaint = complaint,
                                Path = docPath 
                            });
                            }
                        }*/

                    unitWork.Save();
                    scope.Complete();
                    response.Success = true;
                    response.ReturnedId = complaint.Id;
                    }
                }
            catch (Exception ex)
                {
                response.Success = false;
                response.Message = ex.Message;
                }
            return response;
            }

        public ActionResponse Update(int id, NewComplaint updatedComplaint)
            {
            try
                {
                if (updatedComplaint != null)
                    {
                    using (var scope = new TransactionScope())
                        {
                        var complaint = unitWork.ComplaintRepository.GetByID(id);
                        if (complaint != null)
                            {
                            unitWork.ComplaintRepository.Update(complaint);
                            unitWork.Save();
                            scope.Complete();
                            response.Success = true;
                            }
                        }
                    }
                }
            catch (Exception ex)
                {
                response.Success = false;
                response.Message = ex.Message;
                }

            return response;
            }

        public ActionResponse Delete(int id)
            {
            try
                {
                if (id <= 0)
                    {
                    response.Success = false;
                    response.Message = msgHelper.GetCannotBeZeroMessage("Complaint Id");
                    return response;
                    }

                var Complaint = unitWork.ComplaintRepository.GetByID(id);
                if (Complaint != null)
                    {
                    unitWork.ComplaintRepository.Delete(Complaint);
                    unitWork.Save();
                    response.Success = true;
                    response.ReturnedId = id;
                    }
                else
                    {
                    response.Success = false;
                    response.Message = msgHelper.GetNotFoundMessage("Complaint");
                    }
                }
            catch (Exception ex)
                {
                response.Success = false;
                response.Message = ex.Message;
                }
            return response;
            }
        }
    }
