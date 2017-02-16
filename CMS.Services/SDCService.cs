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
        IEnumerable<SDCView> GetAll();

        /// <summary>
        /// Get sdc's for the provided Ids
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        IEnumerable<SDC> GetSDCs(List<int> Ids);

        /// <summary>
        /// Gets list of district SDCs
        /// </summary>
        /// <param name="districtId"></param>
        /// <returns></returns>
        IEnumerable<SDC> GetDistrictSDCs(int districtId);

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

        public IEnumerable<SDCView> GetAll()
            {
            using (var unitWork = new UnitOfWork())
                {
                List<SDCView> sdcsList = new List<SDCView>();
                var sdcs = unitWork.SDCRepository.GetWithInclude(s => s.Id != 0, new string[] { "District" });
                if (sdcs.Any())
                    {
                    foreach(var sdc in sdcs)
                        {
                        sdcsList.Add(new SDCView()
                            {
                                Id = sdc.Id,
                                Title = sdc.Title,
                                District = sdc.District.Name,
                                DistrictId = sdc.District.Id
                            });
                        }
                    return sdcsList;
                    }
                return null;
                }
            }

        public IEnumerable<SDC> GetSDCs(List<int> Ids)
            {
            using (var unitWork = new UnitOfWork())
                {
                List<SDC> sdcList = new List<SDC>();
                var sdcs = unitWork.SDCRepository.GetWithInclude(s => Ids.Contains(s.Id), new string[] {"District"});
                foreach(var sdc in sdcs)
                    {
                    sdcList.Add(new SDC()
                        {
                            Id = sdc.Id,
                            Title = sdc.Title,
                            DistrictId = sdc.District.Id
                        });
                    }
                return sdcList;
                }
            }

        public IEnumerable<SDC> GetDistrictSDCs(int districtId)
            {
            using (var unitWork = new UnitOfWork())
                {
                List<SDC> sdcList = new List<SDC>();
                var sdcs = unitWork.SDCRepository.GetWithInclude(s => s.District.Id == districtId, new string[] { "District" });

                foreach (var sdc in sdcs)
                    {
                    sdcList.Add(new SDC()
                    {
                        Id = sdc.Id,
                        Title = sdc.Title,
                        DistrictId = sdc.District.Id
                    });
                    }
                return sdcList;
                }
            }

        public SDC GetById(int id)
            {
            var SDC = unitWork.SDCRepository.GetByID(id);
            var mapper = config.CreateMapper();
            if (SDC != null)
                {
                var mappedSDC = mapper.Map<EFSDC, SDC>(SDC);
                return mappedSDC;
                }
            return null;
            }

        public ActionResponse Add(SDC newSDC)
            {
            try
                {
                var district = unitWork.DistrictRepository.GetByID(newSDC.DistrictId);
                if (district == null)
                    {
                    response.Success = false;
                    response.Message = msgHelper.GetNotFoundMessage("District");
                    return response;
                    }

                using (var scope = new TransactionScope())
                    {
                    var SDC = new EFSDC()
                    {
                        Title = newSDC.Title,
                        District = district
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
                    var district = unitWork.DistrictRepository.GetByID(updatedSDC.DistrictId);
                    if (district == null)
                        {
                        response.Success = false;
                        response.Message = msgHelper.GetNotFoundMessage("District");
                        return response;
                        }

                    using (var scope = new TransactionScope())
                        {
                        var SDC = unitWork.SDCRepository.GetByID(id);
                        if (SDC != null)
                            {
                            SDC.Title = updatedSDC.Title;
                            SDC.District = district;
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

