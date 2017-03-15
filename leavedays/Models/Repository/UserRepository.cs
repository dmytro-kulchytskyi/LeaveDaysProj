using leavedays.Models.Interfaces.Repository;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using leavedays.Models;

namespace leavedays.Models.Repository
{
    public class UserRepository : IUserRepository
    {
        readonly ISessionFactory sessionFactory;

        public UserRepository(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public IList<AppUser> GetAll()
        {
            throw new NotImplementedException();
        }

        public AppUser GetById(int id)
        {
            using (var sesson = sessionFactory.OpenSession())
            {
                return sesson.Get<AppUser>(id);
            }
        }


        public AppUser GetByUserName(string userName)
        {
            using (var session = sessionFactory.OpenSession())
            {
                var user = session.CreateCriteria<AppUser>()
                    .Add(Restrictions.Eq("UserName", userName))
                    .UniqueResult<AppUser>();
                return user;
            }
        }

        public int Save(AppUser user)
        {
            using (var sesson = sessionFactory.OpenSession())
            {
                using (var t = sesson.BeginTransaction())
                {
                    sesson.SaveOrUpdate(user);
                    t.Commit();
                    return user.Id;
                }
            }
        }
    }
}
