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
    public interface IDistrictService
        {
        /// <summary>
        /// Gets list of all categories
        /// </summary>
        /// <returns></returns>
        IEnumerable<District> GetAll();

        /// <summary>
        /// Gets a District by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        District GetById(int id);

        /// <summary>
        /// Adds a new District
        /// </summary>
        /// <param name="newDistrict"></param>
        /// <returns></returns>
        ActionResponse Add(District newDistrict);

        /// <summary>
        /// Updates the provided District
        /// </summary>
        /// <param name="DistrictId"></param>
        /// <param name="updatedDistrict"></param>
        /// <returns></returns>
        ActionResponse Update(int DistrictId, District updatedDistrict);

        /// <summary>
        /// Deletes the provided District
        /// </summary>
        /// <param name="DistrictId"></param>
        /// <returns></returns>
        ActionResponse Delete(int DistrictId);
        }

    public class DistrictService : IDistrictService
        {
        private readonly UnitOfWork unitWork;
        IMessageHelper msgHelper;
        ActionResponse response;
        MapperConfiguration config;

        public DistrictService()
            {
            unitWork = new UnitOfWork();
            msgHelper = new MessageHelper();
            response = new ActionResponse();
            config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EFDistrict, District>();
            });
            }

        public IEnumerable<District> GetAll()
            {
            using (var unitWork = new UnitOfWork())
                {
                var mapper = config.CreateMapper();
                //Mapper.Initialize(cfg => cfg.CreateMap<EFDistrict, District>());
                List<District> categoriesList = new List<District>();
                var categories = unitWork.DistrictRepository.GetAll().ToList();
                if (categories.Any())
                    {
                    categoriesList = mapper.Map<List<EFDistrict>, List<District>>(categories);
                    return categoriesList;
                    }
                return null;
                }
            }

        public District GetById(int id)
            {
            using (var unitWork = new UnitOfWork())
                {
                var district = unitWork.DistrictRepository.GetByID(id);
                var mapper = config.CreateMapper();
                if (district != null)
                    {
                    var mappedDistrict = mapper.Map<EFDistrict, District>(district);
                    return mappedDistrict;
                    }
                return null;
                }
            }

        public ActionResponse Add(District newDistrict)
            {
            try
                {
                using (var scope = new TransactionScope())
                    {
                    var District = new EFDistrict()
                    {
                        Name = newDistrict.Name
                    };
                    unitWork.DistrictRepository.Insert(District);
                    unitWork.Save();
                    scope.Complete();
                    response.Success = true;
                    response.ReturnedId = District.Id;
                    }
                }
            catch (Exception ex)
                {
                response.Success = false;
                response.Message = ex.Message;
                }
            return response;
            }

        public ActionResponse Update(int id, District updatedDistrict)
            {
            try
                {
                if (updatedDistrict != null)
                    {
                    using (var scope = new TransactionScope())
                        {
                        var District = unitWork.DistrictRepository.GetByID(id);
                        if (District != null)
                            {
                            District.Name = updatedDistrict.Name;
                            unitWork.DistrictRepository.Update(District);
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
                    response.Message = msgHelper.GetCannotBeZeroMessage("District Id");
                    return response;
                    }

                var District = unitWork.DistrictRepository.GetByID(id);
                if (District != null)
                    {
                    unitWork.DistrictRepository.Delete(District);
                    unitWork.Save();
                    response.Success = true;
                    response.ReturnedId = id;
                    }
                else
                    {
                    response.Success = false;
                    response.Message = msgHelper.GetNotFoundMessage("District");
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
