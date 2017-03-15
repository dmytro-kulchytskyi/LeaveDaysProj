using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace leavedays.Models.Repository.Interfaces
{
    public interface ISiteGroupRepository
    {
        SiteGroup GetById(int Id);
        SiteGroup GetByName(string Name);
        int Save(SiteGroup group);

    }
}
