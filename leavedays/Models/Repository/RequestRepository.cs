﻿using leavedays.Models.Repository.Interfaces;
using leavedays.Models.ViewModel;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Models.Repository
{
    public class RequestRepository : IRequestRepository
    {

        readonly ISessionFactory sessionFactory;

        public RequestRepository(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public IEnumerable<ViewRequest> GetByCompanyId(int companyId)
        {
            using (var session = sessionFactory.OpenSession())
            {
                var result = session.CreateCriteria<Request>().
                    Add(Restrictions.Eq("CompanyId", companyId))
                    .SetProjection(Projections.ProjectionList()
                    .Add(Projections.Id(), "Id")
                    .Add(Projections.Property("VacationDates"), "VacationInterval")
                    .Add(Projections.Property("RequestBase"), "RequestBase")
                    .Add(Projections.Property("SigningDate"), "SigningDate")
                    .Add(Projections.Property("IsAccepted"), "IsAccepted"))
                    .SetResultTransformer(Transformers.AliasToBean<ViewRequest>())
                    .List<ViewRequest>();
                return result;
            }
        }

        public Request GetById(int id)
        {
            using (var session = sessionFactory.OpenSession())
            {
                return session.Get<Request>(id);
            }
        }

        public IEnumerable<Request> GetByUserId(int userId)
        {
            using (var session = sessionFactory.OpenSession())
            {
                var result = session.CreateCriteria<Request>().
                    Add(Restrictions.Eq("UserId", userId)).List<Request>();
                return result;
            }
        }

        public int Save(Request request)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.SaveOrUpdate(request);
                    transaction.Commit();
                    return request.Id;
                }
            }
        }
    }
}