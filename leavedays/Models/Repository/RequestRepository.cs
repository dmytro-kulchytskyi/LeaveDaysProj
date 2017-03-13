using leavedays.Models.Repository.Interfaces;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Models.Repository
{
    public class RequestRepository : IRequestRepository
    {

        readonly ISessionFactory sessionFactory;

        RequestRepository(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public IEnumerable<Request> GetByCompanyId(int companyId)
        {
            using (var session = sessionFactory.OpenSession())
            {
                var result = session.CreateCriteria<Request>().
                    Add(Restrictions.Eq("CompanyId", companyId)).List<Request>();
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
                    session.Save(request);
                    transaction.Commit();
                    return request.Id;
                }
            }
        }
    }
}