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
                VacationDates = editRequest.VacationDates,
                IsAccepted = RequestStatus.InProgress
            };
            requestRepository.Save(request);
        }

        public IEnumerable<ViewRequest> GetInProgressRequest(int id)
        {
            return requestRepository.GetByRequestStatus(id, RequestStatus.InProgress);
        }

        public IEnumerable<ViewRequest> GetConfirmedRequest(int id)
        {
            return requestRepository.GetByRequestStatus(id, RequestStatus.Accepted, RequestStatus.NotAccepted);
        }

        public IEnumerable<ViewRequest> GetSendedByUserId(int id)
        {
            return requestRepository.GetByUserIdForView(id);
        }

        public void Accept(int id)
        {
            Request request = requestRepository.GetById(id);
            request.IsAccepted = RequestStatus.Accepted;
            requestRepository.Save(request);
        }

        public void Reject(int id)
        {
            Request request = requestRepository.GetById(id);
            request.IsAccepted = RequestStatus.NotAccepted;
            requestRepository.Save(request);
        }

    }
}