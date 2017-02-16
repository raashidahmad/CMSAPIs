﻿using CMS.DataModel;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Routing;

namespace CMS.Api.Models
    {
    public class ModelFactory
        {
        private UrlHelper _UrlHelper;
        private ApplicationUserManager _AppUserManager;

        public ModelFactory(HttpRequestMessage request, ApplicationUserManager appUserManager)
            {
            _UrlHelper = new UrlHelper(request);
            _AppUserManager = appUserManager;
            }

        public UserReturnModel Create(ApplicationUser appUser)
            {
            return new UserReturnModel
                {
                Url = _UrlHelper.Link("GetUserById", new { id = appUser.Id }),
                Id = appUser.Id,
                UserName = appUser.UserName,
                FullName = string.Format("{0} {1}", appUser.FirstName, appUser.LastName),
                Email = appUser.Email,
                EmailConfirmed = appUser.EmailConfirmed,
                CreationDate = appUser.CreationDate,
                DistrictId = appUser.DistrictId,
                SDCId = appUser.SDCId,
                Mobile = appUser.Mobile,
                Roles = _AppUserManager.GetRolesAsync(appUser.Id).Result
                };
            }

        public RoleReturnModel Create(IdentityRole appRole)
            {
            return new RoleReturnModel
                {
                Url = _UrlHelper.Link("GetRoleById", new { id = appRole.Id }),
                Id = appRole.Id,
                Name = appRole.Name
                };
            }
        }

    public class UserReturnModel
        {
        public string Url { get; set; }
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int SDCId { get; set; }
        public int DistrictId { get; set; }
        public string Mobile { get; set; }
        public bool EmailConfirmed { get; set; }
        public DateTime CreationDate { get; set; }
        public IList<string> Roles { get; set; }
        }

    public class RoleReturnModel
        {
        public string Url { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        }
    }