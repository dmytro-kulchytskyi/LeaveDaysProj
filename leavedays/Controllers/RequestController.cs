using leavedays.Models;
using leavedays.Models.EditModel;
using leavedays.Services;
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

        public RequestController(RequestService requestService)
        {
            this.requestService = requestService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            EditRequest request = new EditRequest
            {
                Status = "New",
                UserId = 1,
                CompanyId = 3
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


    }
}