using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Models
{
    public class User
    {
        public virtual int Id { get; set; }
        public virtual string UserName { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual IEnumerable<SiteGroup> Groups { get; set; }
        public virtual IEnumerable<Module> Modules { get; set; }
    }
}