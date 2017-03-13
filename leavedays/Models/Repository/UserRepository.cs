using leavedays.Models.Interfaces.Repository;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using leavedays.Models;

namespace NewsWebSite.Models.Repository
{
    public class UserRepository : IUserRepository
    {
        readonly ISessionFactory sessionFactory;

        public UserRepository(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public IList<User> GetAll()
        {
            throw new NotImplementedException();
        }

        public User GetById(int id)
        {
            using (var sesson = sessionFactory.OpenSession())
            {
                return sesson.Get<User>(id);
            }
        }


        public User GetByUserName(string userName)
        {
            using (var session = sessionFactory.OpenSession())
            {
                var user = session.CreateCriteria<User>()
                    .Add(Restrictions.Eq("UserName", userName))
                    .UniqueResult<User>();
                return user;
            }
        }

        public int Save(User user)
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
