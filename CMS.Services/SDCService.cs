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
    public interface ISDCService
        {
        /// <summary>
        /// Gets list of all sdcs
        /// </summary>
        /// <returns></returns>
        IEnumerable<SDC> GetAll();

        /// <summary>
        /// Gets a SDC by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        SDC GetById(int id);

        /// <summary>
        /// Adds a new SDC
        /// </summary>
        /// <param name="newSDC"></param>
        /// <returns></returns>
        ActionResponse Add(SDC newSDC);

        /// <summary>
        /// Updates the provided SDC
        /// </summary>
        /// <param name="SDCId"></param>
        /// <param name="updatedSDC"></param>
        /// <returns></returns>
        ActionResponse Update(int SDCId, SDC updatedSDC);

        /// <summary>
        /// Deletes the provided SDC
        /// </summary>
        /// <param name="SDCId"></param>
        /// <returns></returns>
        ActionResponse Delete(int SDCId);
        }

    public class SDCService : ISDCService
        {
        private readonly UnitOfWork unitWork;
        IMessageHelper msgHelper;
        ActionResponse response;
        MapperConfiguration config;

        public SDCService()
            {
            unitWork = new UnitOfWork();
            msgHelper = new MessageHelper();
            response = new ActionResponse();
            config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EFSDC, SDC>();
            });
            }

        public IEnumerable<SDC> GetAll()
            {
            using (var unitWork = new UnitOfWork())
                {
                var mapper = config.CreateMapper();
                List<SDC> sdcsList = new List<SDC>();
                var sdcs = unitWork.SDCRepository.GetAll().ToList();
                if (sdcs.Any())
                    {
                    sdcsList = mapper.Map<List<EFSDC>, List<SDC>>(sdcs);
                    return sdcsList;
                    }
                return null;
                }
            }

        public SDC GetById(int id)
            {
            var SDC = unitWork.SDCRepository.GetByID(id);
            if (SDC != null)
                {
                var mappedSDC = Mapper.Map<EFSDC, SDC>(SDC);
                return mappedSDC;
                }
            return null;
            }

        public ActionResponse Add(SDC newSDC)
            {
            try
                {
                using (var scope = new TransactionScope())
                    {
                    var SDC = new EFSDC()
                    {
                        Title = newSDC.Title
                    };
                    unitWork.SDCRepository.Insert(SDC);
                    unitWork.Save();
                    scope.Complete();
                    response.Success = true;
                    response.ReturnedId = SDC.Id;
                    }
                }
            catch (Exception ex)
                {
                response.Success = false;
                response.Message = ex.Message;
                }
            return response;
            }

        public ActionResponse Update(int id, SDC updatedSDC)
            {
            try
                {
                if (updatedSDC != null)
                    {
                    using (var scope = new TransactionScope())
                        {
                        var SDC = unitWork.SDCRepository.GetByID(id);
                        if (SDC != null)
                            {
                            SDC.Title = updatedSDC.Title;
                            unitWork.SDCRepository.Update(SDC);
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
                    response.Message = msgHelper.GetCannotBeZeroMessage("SDC Id");
                    return response;
                    }

                var SDC = unitWork.SDCRepository.GetByID(id);
                if (SDC != null)
                    {
                    unitWork.SDCRepository.Delete(SDC);
                    unitWork.Save();
                    response.Success = true;
                    response.ReturnedId = id;
                    }
                else
                    {
                    response.Success = false;
                    response.Message = msgHelper.GetNotFoundMessage("SDC");
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

