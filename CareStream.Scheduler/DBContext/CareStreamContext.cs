using System;
using System.Collections.Generic;
using System.Text;
using CareStream.Models;
using Microsoft.EntityFrameworkCore;

namespace CareStream.Scheduler
{
    public class CareStreamContext : DbContext
    {
        public CareStreamContext(DbContextOptions<CareStreamContext> options) : base(options)
        {

        }
        public DbSet<BulkUserFile> BulkUserFiles { get; set; }
        public DbSet<BulkUser> BulkUsers { get; set; }
    }
}
