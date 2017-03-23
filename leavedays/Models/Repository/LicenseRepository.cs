using leavedays.Models.Repository.Interfaces;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Models.Repository
{
    public class LicenseRepository : ILicenseRepository
    {
        readonly ISessionFactory sessionFactory;

        public LicenseRepository(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public IList<License> GetAll()
        {
            using (var session = sessionFactory.OpenSession())
            {
                var results = session.CreateCriteria<License>().List<License>();
                return results;
            }
        }

        public License GetById(int id)
        {
            using (var session = sessionFactory.OpenSession())
            {
                return session.Get<License>(id);
            }
        }

        public License GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public int Save(License license)
        {
            throw new NotImplementedException();
        }
    }
}