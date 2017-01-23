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
    
    public interface ICategoryService
        {
        /// <summary>
        /// Gets list of all categories
        /// </summary>
        /// <returns></returns>
        IEnumerable<Category> GetAll();

        /// <summary>
        /// Gets a Category by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Category GetById(int id);

        /// <summary>
        /// Adds a new Category
        /// </summary>
        /// <param name="newCategory"></param>
        /// <returns></returns>
        ActionResponse Add(Category newCategory);

        /// <summary>
        /// Updates the provided Category
        /// </summary>
        /// <param name="CategoryId"></param>
        /// <param name="updatedCategory"></param>
        /// <returns></returns>
        ActionResponse Update(int CategoryId, Category updatedCategory);

        /// <summary>
        /// Deletes the provided Category
        /// </summary>
        /// <param name="CategoryId"></param>
        /// <returns></returns>
        ActionResponse Delete(int CategoryId);
        }
    
    public class CategoryService : ICategoryService
        {
        private readonly UnitOfWork unitWork;
        IMessageHelper msgHelper;
        ActionResponse response;
        MapperConfiguration config;

        public CategoryService()
            {
            unitWork = new UnitOfWork();
            msgHelper = new MessageHelper();
            response = new ActionResponse();
            config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EFCategory, Category>();
            });
            }

        public IEnumerable<Category> GetAll()
            {
            using (var unitWork = new UnitOfWork())
                {
                var mapper = config.CreateMapper();
                List<Category> categoriesList = new List<Category>();
                var categories = unitWork.CategoryRepository.GetAll().ToList();
                if (categories.Any())
                    {
                    categoriesList = mapper.Map<List<EFCategory>, List<Category>>(categories);
                    return categoriesList;
                    }
                return null;
                }
            }

        public Category GetById(int id)
            {
            using (var unitWork = new UnitOfWork())
                {
                var mapper = config.CreateMapper();
                var category = unitWork.CategoryRepository.GetByID(id);
                if (category != null)
                    {
                    var mappedCategory = mapper.Map<EFCategory, Category>(category);
                    return mappedCategory;
                    }
                return null;
                }
            }

        public ActionResponse Add(Category newCategory)
            {
            try
                {
                using (var scope = new TransactionScope())
                    {
                    var Category = new EFCategory()
                    {
                        Name = newCategory.Name
                    };
                    unitWork.CategoryRepository.Insert(Category);
                    unitWork.Save();
                    scope.Complete();
                    response.Success = true;
                    response.ReturnedId = Category.Id;
                    }
                }
            catch (Exception ex)
                {
                response.Success = false;
                response.Message = ex.Message;
                }
            return response;
            }

        public ActionResponse Update(int id, Category updatedCategory)
            {
            try
                {
                if (updatedCategory != null)
                    {
                    using (var scope = new TransactionScope())
                        {
                        var Category = unitWork.CategoryRepository.GetByID(id);
                        if (Category != null)
                            {
                            Category.Name = updatedCategory.Name;
                            unitWork.CategoryRepository.Update(Category);
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
                    response.Message = msgHelper.GetCannotBeZeroMessage("Category Id");
                    return response;
                    }

                var Category = unitWork.CategoryRepository.GetByID(id);
                if (Category != null)
                    {
                    unitWork.CategoryRepository.Delete(Category);
                    unitWork.Save();
                    response.Success = true;
                    response.ReturnedId = id;
                    }
                else
                    {
                    response.Success = false;
                    response.Message = msgHelper.GetNotFoundMessage("Category");
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
