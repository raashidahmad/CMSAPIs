namespace CMS.DataModel.Migrations
    {
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CMS.DataModel.CMDbContext>
        {
        public Configuration()
            {
            AutomaticMigrationsEnabled = true;
            }

        protected override void Seed(CMS.DataModel.CMDbContext context)
            {

            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var user = new ApplicationUser()
            {
                UserName = "RashidAhmad",
                PublicUserId = "RashidAhmad",
                Email = "raashid.ahmad@gmail.com",
                EmailConfirmed = true,
                FirstName = "Rashid",
                LastName = "Ahmad",
                CreationDate = DateTime.Now.AddYears(-3),
                Mobile = "03002133445",
                SDCId = 1
            };

            manager.Create(user, "MySuperP@ss!");
            if (roleManager.Roles.Count() == 0)
                {
                roleManager.Create(new IdentityRole { Name = "SuperAdmin" });
                roleManager.Create(new IdentityRole { Name = "Admin" });
                roleManager.Create(new IdentityRole { Name = "Operator" });
                }

            var adminUser = manager.FindByName("RashidAhmad");
            manager.AddToRoles(adminUser.Id, new string[] { "SuperAdmin", "Admin" });
            context.SaveChanges();

            //Adding Some Districts
            context.Districts.Add(new Models.EFDistrict()
            {
                Name = "Peshawar",
                SMSPrefix = "PSH"
            });
            context.Districts.Add(new Models.EFDistrict()
            {
                Name = "Mardan",
                SMSPrefix = "MRD"
            });
            context.SaveChanges();
            }
        }
    }
