using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Text;
using System.Threading.Tasks;
using CMS.DataModel.Models;

namespace CMS.DataModel
{
    public class CMDbContext : ApplicationDbContext
    {
        public CMDbContext(IDatabaseInitializer<CMDbContext> dbInitializer)
            : base()
            {
            if (dbInitializer != null)
                Database.SetInitializer(dbInitializer);
            }

        public CMDbContext()
            : this(new MigrateDatabaseToLatestVersion())
            {
            }

        public DbSet<EFDistrict> Districts { get; set; }
        public DbSet<EFCategory> Categories { get; set; }
        public DbSet<EFSDC> SDCS { get; set; }
        public DbSet<EFComplainant> Complainants { get; set; }
        public DbSet<EFComplaint> Complaints { get; set; }
        public DbSet<EFSMSLogs> SMSLogs { get; set; }
        public DbSet<EFPhoneCallLogs> PhoneLogs { get; set; }

    }
}
