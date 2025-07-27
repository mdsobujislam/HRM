using HRM.Interfaces;
using HRM.Models;
using HRM.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRM.Controllers
{
    public class AddUserController : Controller
    {
        private readonly IAddUserService _addUser;
        private readonly IBranchService _branchService;
        public AddUserController(IAddUserService addUser, IBranchService branchService)
        {
            _addUser = addUser ?? throw new ArgumentNullException(nameof(addUser));
            _branchService = branchService;
        }
        //public async Task<IActionResult> Index()
        //{
        //    var branches = await _branchService.GetAllBranch();
        //    ViewBag.BranchList = branches.Select(b => new SelectListItem
        //    {
        //        Value = b.Id.ToString(),
        //        Text = b.Name
        //    }).ToList();

        //    var users = await _addUser.GetAllAddUser();
        //    return View(new AddUser());
        //}

        public async Task<IActionResult> Index()
        {
            var users = await _addUser.GetAllAddUser(); // List<AddUser>
            var branches = await _branchService.GetAllBranch();

            ViewBag.BranchList = branches.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name
            }).ToList();

            var viewModel = new UserListViewModel
            {
                UserList = users,
                NewUser = new AddUser()
            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateUser(AddUser user)
        {
            if (user.Id == 0)
            {
                bool isCreated = await _addUser.InsertAddUser(user);
                if (!isCreated)
                {
                    TempData["ErrorMessage"] = "A User already exists for this SubscriptionId.";
                    return RedirectToAction("INdex");
                }
                TempData["SuccessMessage"] = "User Created Successfully";
            }
            else
            {
                bool isUpdated = await _addUser.UpdateAddUser(user);
                if (!isUpdated)
                {
                    TempData["ErrorMessage"] = "User name already exists or update failed";
                    return RedirectToAction("Index");
                }
                TempData["SuccessMessage"] = "User Updated Successfully";
            }
            return RedirectToAction("Index");
        }

        //[HttpPost]
        //public async Task<IActionResult> ToggleUserStatus(int userId)
        //{
        //    bool success = await _addUser.ToggleUserStatusAsync(userId);

        //    if (!success)
        //        return NotFound();

        //    return RedirectToAction("UserList");
        //}
    }
}
