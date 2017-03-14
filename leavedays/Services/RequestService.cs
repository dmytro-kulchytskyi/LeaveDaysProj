using leavedays.Models;
using leavedays.Models.EditModel;
using leavedays.Models.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace leavedays.Services
{
    public class RequestService
    {
        private readonly IRequestRepository requestRepository;
        public RequestService(IRequestRepository requestRepository)
        {
            this.requestRepository = requestRepository;
        }

        public void Save(EditRequest editRequest)
        {
            Request request = new Request
            {
                UserId = editRequest.UserId,
                CompanyId = editRequest.CompanyId,
                Status = editRequest.Status,
                RequestBase = editRequest.RequestBase,
                SigningDate = DateTime.Now,
                VacationDates = editRequest.VacationDates
            };
            requestRepository.Save(request);
        }

    }
}