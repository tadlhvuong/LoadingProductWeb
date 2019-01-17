using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TCVShared.Data;
using TCVShared.Helpers;
using TCVWeb.Areas.Admin.Models;
using TCVWeb.Models;

namespace TCVWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Manager")]
    public class MemberController : Controller
    {
        private readonly ILogger _logger;
        private readonly AppDBContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public MemberController(
            ILogger<MemberController> logger,
            AppDBContext dbContext,
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index(PagedList<AppUser> model)
        {
            var filterQuery = _dbContext.Users.Where(x => model.Search == null || x.UserName.Contains(model.Search));

            if (model.Filter == null)
                filterQuery = filterQuery.Where(x => x.Status != EntityStatus.Expiried);
            else
            {
                if (int.TryParse(model.Filter, out int status))
                {
                    EntityStatus etStatus = (EntityStatus)status;
                    filterQuery = filterQuery.Where(x => x.Status == etStatus);
                }
            }

            var selectQuery = filterQuery.OrderByDescending(x => x.CreateTime).Skip((model.CurPage - 1) * model.PageSize).Take(model.PageSize);

            model.TotalRows = filterQuery.Count();
            model.Content = selectQuery.ToList();

            return View(model);
        }

        public IActionResult Details(int id)
        {
            AppUser model = _dbContext.Users.Find(id);
            if (model == null)
                return NotFound();

            return View(model);
        }

        public IActionResult Update(int id)
        {
            AppUser appUser = _dbContext.Users.Find(id);
            if (appUser == null)
                return NotFound();

            UpdateUserModel model = new UpdateUserModel()
            {
                UserId = appUser.Id,
                UserName = appUser.UserName,
                Email = appUser.Email,
                PhoneNumber = appUser.PhoneNumber,
                Status = appUser.Status,
            };

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateUserModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AppUser appUser = _dbContext.Users.Find(model.UserId);
                    if (appUser == null)
                        return NotFound();

                    appUser.Email = model.Email;
                    appUser.EmailConfirmed = !string.IsNullOrEmpty(model.Email);

                    appUser.PhoneNumber = model.PhoneNumber;
                    appUser.PhoneNumberConfirmed = !string.IsNullOrEmpty(model.PhoneNumber);

                    appUser.LastUpdate = DateTime.Now;
                    appUser.UpdateUser = User.Identity.Name;

                    appUser.Status = model.Status;

                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        await _userManager.RemovePasswordAsync(appUser);
                        await _userManager.AddPasswordAsync(appUser, model.Password);
                    }

                    _dbContext.SaveChanges();
                    return Json(new ModalFormResult() { Code = 1 });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return View(model);
        }

        

        public IActionResult Search()
        {
            MemberSearchModel model = new MemberSearchModel() { FindMode = 1 };
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Search(MemberSearchModel model)
        {
            if (ModelState.IsValid)
            {
                bool doSearch = false;
                IQueryable<AppUser> usersQuery = _dbContext.Users;

                if (!string.IsNullOrEmpty(model.UserName))
                {
                    doSearch = true;
                    if (model.FindMode == 1)
                        usersQuery = usersQuery.Where(u => u.UserName.Equals(model.UserName));
                    else if (model.FindMode == 2)
                        usersQuery = usersQuery.Where(u => u.UserName.StartsWith(model.UserName));
                    else
                        usersQuery = usersQuery.Where(u => u.UserName.Contains(model.UserName));
                }

                if (!string.IsNullOrEmpty(model.Email))
                {
                    doSearch = true;
                    if (model.FindMode == 1)
                        usersQuery = usersQuery.Where(u => u.Email.Equals(model.Email));
                    else if (model.FindMode == 2)
                        usersQuery = usersQuery.Where(u => u.Email.StartsWith(model.Email));
                    else
                        usersQuery = usersQuery.Where(u => u.Email.Contains(model.Email));
                }

                if (!string.IsNullOrEmpty(model.PhoneNumber))
                {
                    doSearch = true;
                    if (model.FindMode == 1)
                        usersQuery = usersQuery.Where(u => u.PhoneNumber.Equals(model.PhoneNumber));
                    else if (model.FindMode == 2)
                        usersQuery = usersQuery.Where(u => u.PhoneNumber.StartsWith(model.PhoneNumber));
                    else
                        usersQuery = usersQuery.Where(u => u.PhoneNumber.Contains(model.PhoneNumber));
                }

                model.Results = doSearch ? usersQuery.OrderBy(u => u.UserName).Take(200).ToList() : null;
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetRole(string userName, string roleName)
        {
            var appUser = await _userManager.FindByNameAsync(userName);
            if (appUser == null)
                return NotFound();

            var roleExist = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
                await _roleManager.CreateAsync(new AppRole(roleName));

            await _userManager.AddToRoleAsync(appUser, roleName);

            return RedirectToAction(nameof(Index));
        }

        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> SetRole(string id, string roleName)
        //{
        //    AppUser appUser = await _userManager.FindByIdAsync(id);
        //    if (appUser == null)
        //        return NotFound();

        //    var roleExist = await _roleManager.RoleExistsAsync(roleName);
        //    if (!roleExist)
        //        await _roleManager.CreateAsync(new IdentityRole(roleName));

        //    await _userManager.AddToRoleAsync(appUser, roleName);

        //    return RedirectToAction(nameof(Index));
        //}
    }
}