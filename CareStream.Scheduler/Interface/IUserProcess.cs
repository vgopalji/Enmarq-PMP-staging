using CareStream.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CareStream.Scheduler
{
    public interface IUserProcess
    {
        public  Task ProcessUser(List<BulkUser> bulkUsers);
    }
}
