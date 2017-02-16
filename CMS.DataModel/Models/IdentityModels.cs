using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DataModel
    {
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
        {
        [Required]
        public string PublicUserId { get; set; }
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        public int DistrictId { get; set; }
        [Required]
        public int SDCId { get; set; }

        public string Mobile { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
            {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
            }
        }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
        {
        public ApplicationDbContext()
            : base("CMS", throwIfV1Schema: false)
            {
            }

        public static ApplicationDbContext Create()
            {
            return new ApplicationDbContext();
            }
        }
    }
