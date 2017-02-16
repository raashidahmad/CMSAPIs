using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using CMS.Api.Providers;
using CMS.Api.Results;
using CMS.DataModel;
using CMS.DataModel.ModelWrapper;
using CMS.Services;

namespace CMS.Api.Controllers
{
    [RoutePrefix("api/accounts")]
    public class AccountsController : BaseApiController
        {

        //[Authorize(Roles = "Admin")]
        [Route("users")]
        public IHttpActionResult GetUsers()
            {
            //Only SuperAdmin or Admin can delete users (Later when implement roles)
            var identity = User.Identity as System.Security.Claims.ClaimsIdentity;
            var users = (this.AppUserManager.Users.ToList().Select(u => this.TheModelFactory.Create(u)));
            List<UserView> usersList = new List<UserView>();
            
            foreach(var user in users)
                {
                usersList.Add(new UserView()
                    {
                        Username = user.UserName,
                        DistrictId = user.DistrictId,
                        SDCId = user.SDCId,
                        FullName = user.FullName,
                        Roles = user.Roles.ToList()
                    });
                }
            return Ok(usersList);
            }

        [Route("roles")]
        public IHttpActionResult GetRoles()
            {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            List<Role> rolesList = new List<Role>();
            //var roles = this.AppRoleManager.Roles.ToList().Select(r => this.TheModelFactory.Create(r));
            var roles = dbContext.Roles.OrderBy(x => x.Name);

            foreach(var role in roles)
                {
                rolesList.Add(new Role()
                    {
                        Id = role.Id,
                        Name = role.Name
                    });
                }
            return Ok(rolesList);
            }

        [Route("usersInRole/{role}")]
        public IHttpActionResult GetUsersInRole(string role)
            {
            //Only SuperAdmin or Admin can delete users (Later when implement roles)
            var identity = User.Identity as System.Security.Claims.ClaimsIdentity;
            var users = this.AppUserManager.Users.ToList().Select(u => this.TheModelFactory.Create(u));
            var selectedUsers = from user in users
                                where user.Roles.Contains(role)
                                select user;
            return Ok(selectedUsers);
            }

        //[Authorize(Roles = "Admin")]
        [Route("user/{id:guid}", Name = "GetUserById")]
        public async Task<IHttpActionResult> GetUser(string Id)
            {
            //Only SuperAdmin or Admin can delete users (Later when implement roles)
            var user = await this.AppUserManager.FindByIdAsync(Id);

            if (user != null)
                {
                return Ok(this.TheModelFactory.Create(user));
                }

            return NotFound();

            }

        //[Authorize(Roles = "Admin")]
        [Route("user/{username}")]
        public async Task<IHttpActionResult> GetUserByName(string username)
            {
            //Only SuperAdmin or Admin can delete users (Later when implement roles)
            var user = await this.AppUserManager.FindByNameAsync(username);

            if (user != null)
                {
                return Ok(this.TheModelFactory.Create(user));
                }

            return NotFound();

            }

        [AllowAnonymous]
        [Route("create")]
        public async Task<IHttpActionResult> CreateUser(CreateUserBindingModel createUserModel)
            {
            if (!ModelState.IsValid)
                {
                return BadRequest(ModelState);
                }
            var user = new ApplicationUser()
            {
                UserName = createUserModel.Username,
                PublicUserId = createUserModel.Username,
                Email = createUserModel.Email,
                FirstName = createUserModel.FirstName,
                LastName = createUserModel.LastName,
                DistrictId = createUserModel.DistrictId,
                SDCId = createUserModel.SDCId,
                CreationDate = DateTime.Now.Date,
            };

            IdentityResult addUserResult = await this.AppUserManager.CreateAsync(user, createUserModel.Password);
            if (!addUserResult.Succeeded)
                {
                return GetErrorResult(addUserResult);
                }

            string code = await this.AppUserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            var callbackUrl = new Uri(Url.Link("ConfirmEmailRoute", new { userId = user.Id, code = code }));
            await this.AppUserManager.SendEmailAsync(user.Id,
                                                    "Confirm your account",
                                                    "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

            Uri locationHeader = new Uri(Url.Link("GetUserById", new { id = user.Id }));
            return Created(locationHeader, TheModelFactory.Create(user));
            }

        [AllowAnonymous]
        [HttpGet]
        [Route("ConfirmEmail", Name = "ConfirmEmailRoute")]
        public async Task<IHttpActionResult> ConfirmEmail(string userId = "", string code = "")
            {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
                {
                ModelState.AddModelError("", "User Id and Code are required");
                return BadRequest(ModelState);
                }

            IdentityResult result = await this.AppUserManager.ConfirmEmailAsync(userId, code);
            if (result.Succeeded)
                {
                return Ok();
                }
            else
                {
                return GetErrorResult(result);
                }
            }

        //[Authorize]
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
            {
            //var user = await this.AppUserManager.FindByIdAsync(model.UserId);
            if (!ModelState.IsValid)
                {
                return BadRequest(ModelState);
                }

            var token = await this.AppUserManager.GeneratePasswordResetTokenAsync(model.UserId);
            var result = await this.AppUserManager.ResetPasswordAsync(model.UserId, token, model.NewPassword);
            if (!result.Succeeded)
                {
                return GetErrorResult(result);
                }
            return Ok();
            }

        //[Authorize(Roles = "Admin")]
        [Route("user/{id:guid}")]
        public async Task<IHttpActionResult> DeleteUser(string id)
            {
            //Only SuperAdmin or Admin can delete users (Later when implement roles)
            var appUser = await this.AppUserManager.FindByIdAsync(id);
            if (appUser != null)
                {
                IdentityResult result = await this.AppUserManager.DeleteAsync(appUser);
                if (!result.Succeeded)
                    {
                    return GetErrorResult(result);
                    }
                return Ok();
                }
            return NotFound();
            }

        //[Authorize(Roles = "Admin")]
        [Route("user/{id:guid}/roles")]
        [HttpPut]
        public async Task<IHttpActionResult> AssignRolesToUser([FromUri] string id, [FromBody] string[] rolesToAssign)
            {

            var appUser = await this.AppUserManager.FindByIdAsync(id);

            if (appUser == null)
                {
                return NotFound();
                }

            var currentRoles = await this.AppUserManager.GetRolesAsync(appUser.Id);
            var rolesNotExists = rolesToAssign.Except(this.AppRoleManager.Roles.Select(x => x.Name)).ToArray();

            if (rolesNotExists.Count() > 0)
                {
                ModelState.AddModelError("", string.Format("Roles '{0}' does not exixts in the system", string.Join(",", rolesNotExists)));
                return BadRequest(ModelState);
                }

            IdentityResult removeResult = await this.AppUserManager.RemoveFromRolesAsync(appUser.Id, currentRoles.ToArray());
            if (!removeResult.Succeeded)
                {
                ModelState.AddModelError("", "Failed to remove user roles");
                return BadRequest(ModelState);
                }
            IdentityResult addResult = await this.AppUserManager.AddToRolesAsync(appUser.Id, rolesToAssign);
            if (!addResult.Succeeded)
                {
                ModelState.AddModelError("", "Failed to add user roles");
                return BadRequest(ModelState);
                }
            return Ok();
            }

        }
}
