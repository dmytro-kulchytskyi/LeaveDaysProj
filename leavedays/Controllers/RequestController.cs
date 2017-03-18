using leavedays.Models;
using leavedays.Models.EditModel;
using leavedays.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace leavedays.Controllers
{
    public class RequestController : Controller
    {
        readonly RequestService requestService;
        private readonly UserManager<AppUser, int> userManager;

        public RequestController(RequestService requestService, UserManager<AppUser, int> userManager)
        {
            this.requestService = requestService;
            this.userManager = userManager;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var currentUser = await userManager.FindByIdAsync(User.Identity.GetUserId<int>());
            EditRequest request = new EditRequest()
            {
                Status = "New",
                UserId = currentUser.Id,
                CompanyId = currentUser.CompanyId
            };
            return View(request);
        }

        [HttpPost]
        public ActionResult Create(EditRequest request)
        {
            if (!ModelState.IsValid) RedirectToAction("Index", "Home");
            requestService.Save(request);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<ActionResult> ConfirmNew()
        {
            var currentUser = await userManager.FindByIdAsync(User.Identity.GetUserId<int>());
            if (await userManager.IsInRoleAsync(currentUser.Id, "FinanceAdmin"))
            {
                return View("RequestPanel", requestService.GetInProgressRequest(currentUser.CompanyId).OrderBy(model => model.IsAccepted));
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult ConfirmNew(int Id, string acceptBtn)
        {
            if (acceptBtn == "Accept")
            {
                requestService.Accept(Id);
                return RedirectToAction("ConfirmNew");
            }
            requestService.Reject(Id);
            return RedirectToAction("ConfirmNew");
        }

        [HttpGet]
        public async Task<ActionResult> ShowConfirmed()
        {
            var currentUser =await userManager.FindByIdAsync(User.Identity.GetUserId<int>());
            if(await userManager.IsInRoleAsync(currentUser.Id, "FinanceAdmin"))
            {
                return View("ConfirmedRequest", requestService.GetConfirmedRequest(currentUser.CompanyId).OrderBy(model => model.IsAccepted));
            }
            return View("Index", "Home");
        }
    }
}