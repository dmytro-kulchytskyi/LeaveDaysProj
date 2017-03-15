using leavedays.Models;
using leavedays.Models.EditModel;
using leavedays.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public ActionResult Create()
        {
            AppUser currentUser = userManager.FindById(User.Identity.GetUserId<int>());
            EditRequest request = new EditRequest
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
        public ActionResult Confirm()
        {
            AppUser currentUser = userManager.FindById(User.Identity.GetUserId<int>());
            if (userManager.IsInRole(currentUser.Id, "Admin"))
            {
                return View("RequestPanel", requestService.GetByCompanyId(currentUser.CompanyId).OrderBy(model => model.IsAccepted));
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Confirm(int Id, string acceptBtn)
        {
            if (acceptBtn == "Accept")
            {
                requestService.Accept(Id);
                return RedirectToAction("Confirm");
            }
            requestService.Reject(Id);
            return RedirectToAction("Confirm");
        }

    }
}