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
        IEnumerable<Complaint> GetAll();

        /// <summary>
        /// Gets a Complaint by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Complaint GetById(int id);

        /// <summary>
        /// Adds a new complaint along with uploaded documents
        /// </summary>
        /// <param name="newCompliant"></param>
        /// <param name="documents"></param>
        /// <returns></returns>
        ActionResponse Add(Complaint newCompliant, List<string> documents);

        /// <summary>
        /// Updates the provided Complaint
        /// </summary>
        /// <param name="ComplaintId"></param>
        /// <param name="updatedComplaint"></param>
        /// <returns></returns>
        ActionResponse Update(int complaintId, Complaint updatedComplaint);

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
        MapperConfiguration config;

        public ComplaintService()
            {
            unitWork = new UnitOfWork();
            msgHelper = new MessageHelper();
            response = new ActionResponse();
            config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EFComplaint, Complaint>();
            });
            }

        public IEnumerable<Complaint> GetAll()
            {
            using (var unitWork = new UnitOfWork())
                {
                var mapper = config.CreateMapper();
                List<Complaint> complaintsList = new List<Complaint>();
                var complaints = unitWork.ComplaintRepository.GetAll().ToList();
                if (complaints.Any())
                    {
                    complaintsList = mapper.Map<List<EFComplaint>, List<Complaint>>(complaints);
                    return complaintsList;
                    }
                return null;
                }
            }

        public Complaint GetById(int id)
            {
            var Complaint = unitWork.ComplaintRepository.GetByID(id);
            if (Complaint != null)
                {
                var mappedComplaint = Mapper.Map<EFComplaint, Complaint>(Complaint);
                return mappedComplaint;
                }
            return null;
            }

        public ActionResponse Add(Complaint newComplaint, List<string> documents)
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
                        SDC = sdc,
                        Description = newComplaint.Description,
                        Dated = DateTime.Now,
                        Status = EFComplaint.ComplaintStatus.InQueue
                    };

                    unitWork.ComplaintRepository.Insert(complaint);
                    //Store references to all the uploaded documents against a complaint
                    if (documents.Count > 0)
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
                        }

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

        public ActionResponse Update(int id, Complaint updatedComplaint)
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
