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
    public interface IComplainantService
        {
        /// <summary>
        /// Gets list of all Complainants
        /// </summary>
        /// <returns></returns>
        IEnumerable<Complainant> GetAll();

        /// <summary>
        /// Gets a Complainant by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Complainant GetById(int id);

        /// <summary>
        /// Adds a new Complainant
        /// </summary>
        /// <param name="newComplainant"></param>
        /// <returns></returns>
        ActionResponse Add(Complainant newComplainant);

        /// <summary>
        /// Updates the provided Complainant
        /// </summary>
        /// <param name="complainantId"></param>
        /// <param name="updatedComplainant"></param>
        /// <returns></returns>
        ActionResponse Update(int complainantId, Complainant updatedComplainant);

        /// <summary>
        /// Deletes the provided Complainant
        /// </summary>
        /// <param name="complainantId"></param>
        /// <returns></returns>
        ActionResponse Delete(int complainantId);
        }

    public class ComplainantService : IComplainantService
        {
        private readonly UnitOfWork unitWork;
        IMessageHelper msgHelper;
        ActionResponse response;
        MapperConfiguration config;

        public ComplainantService()
            {
            unitWork = new UnitOfWork();
            msgHelper = new MessageHelper();
            response = new ActionResponse();
            config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EFComplainant, Complainant>();
            });
            }

        public IEnumerable<Complainant> GetAll()
            {
            using (var unitWork = new UnitOfWork())
                {
                var mapper = config.CreateMapper();
                List<Complainant> complainantsList = new List<Complainant>();
                var Complainants = unitWork.ComplainantRepository.GetAll().ToList();
                if (Complainants.Any())
                    {
                    complainantsList = mapper.Map<List<EFComplainant>, List<Complainant>>(Complainants);
                    return complainantsList;
                    }
                return null;
                }
            }

        public Complainant GetById(int id)
            {
            var complainant = unitWork.ComplainantRepository.GetByID(id);
            if (complainant != null)
                {
                var mappedComplainant = Mapper.Map<EFComplainant, Complainant>(complainant);
                return mappedComplainant;
                }
            return null;
            }

        public ActionResponse Add(Complainant newComplainant)
            {
            try
                {
                using (var scope = new TransactionScope())
                    {
                    var Complainant = new EFComplainant()
                    {
                        FullName = newComplainant.FullName,
                        NIC = newComplainant.NIC,
                        Mobile = newComplainant.Mobile,
                        ContactMedium = (EFComplainant.CommunicationMedium)newComplainant.ContactMedium,
                        Address = newComplainant.Address,
                        
                    };
                    unitWork.ComplainantRepository.Insert(Complainant);
                    unitWork.Save();
                    scope.Complete();
                    response.Success = true;
                    response.ReturnedId = Complainant.Id;
                    }
                }
            catch (Exception ex)
                {
                response.Success = false;
                response.Message = ex.Message;
                }
            return response;
            }

        public ActionResponse Update(int id, Complainant updatedComplainant)
            {
            try
                {
                if (updatedComplainant != null)
                    {
                    using (var scope = new TransactionScope())
                        {
                        var complainant = unitWork.ComplainantRepository.GetByID(id);
                        if (complainant != null)
                            {
                            complainant.FullName = updatedComplainant.FullName;
                            complainant.NIC = updatedComplainant.NIC;
                            complainant.Mobile = updatedComplainant.Mobile;
                            complainant.ContactMedium = (EFComplainant.CommunicationMedium)updatedComplainant.ContactMedium;
                            complainant.Address = updatedComplainant.Address;
                            unitWork.ComplainantRepository.Update(complainant);
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
                    response.Message = msgHelper.GetCannotBeZeroMessage("Complainant Id");
                    return response;
                    }

                var Complainant = unitWork.ComplainantRepository.GetByID(id);
                if (Complainant != null)
                    {
                    unitWork.ComplainantRepository.Delete(Complainant);
                    unitWork.Save();
                    response.Success = true;
                    response.ReturnedId = id;
                    }
                else
                    {
                    response.Success = false;
                    response.Message = msgHelper.GetNotFoundMessage("Complainant");
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
