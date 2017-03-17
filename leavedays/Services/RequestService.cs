using leavedays.Models;
using leavedays.Models.EditModel;
using leavedays.Models.Repository.Interfaces;
using leavedays.Models.ViewModel;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace leavedays.Services
{
    public class RequestService
    {
        private readonly IRequestRepository requestRepository;
        private readonly IUserRepository userRepository;
        public RequestService(IRequestRepository requestRepository, IUserRepository userRepository)
        {
            this.requestRepository = requestRepository;
            this.userRepository = userRepository;
        }

        public void Save(EditRequest editRequest)
        {
            Request request = new Request
            {
                User = userRepository.GetById(editRequest.UserId),
                CompanyId = editRequest.CompanyId,
                Status = editRequest.Status,
                RequestBase = editRequest.RequestBase,
                SigningDate = DateTime.Now,
                VacationDates = editRequest.VacationDates
            };
            requestRepository.Save(request);
        }

        public IEnumerable<ViewRequest> GetByCompanyId(int id)
        {
            return requestRepository.GetByCompanyId(id);
        }

        public void Accept(int id)
        {
            Request request = requestRepository.GetById(id);
            request.IsAccepted = true;
            requestRepository.Save(request);
        }

        public void Reject(int id)
        {
            Request request = requestRepository.GetById(id);
            request.IsAccepted = false;
            requestRepository.Save(request);
        }

    }
}