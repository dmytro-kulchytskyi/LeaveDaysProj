using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Models
{
    public class Company
    {
        public virtual int Id { get; set; }
        public virtual string FullName { get; set; }
        public virtual string UrlName { get; set; }
    }
}