using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DataModel
    {
    class DropCreateDatabaseAlways : DropCreateDatabaseAlways<CMDbContext>
        {
        public override void InitializeDatabase(CMDbContext context)
            {
            base.InitializeDatabase(context);
            }

        protected override void Seed(CMDbContext context)
            {
            DataSeeder.Seed(context);
            base.Seed(context);
            }
        }
    class CreateDatabaseIfNotExists : CreateDatabaseIfNotExists<CMDbContext>
        {
        public override void InitializeDatabase(CMDbContext context)
            {
            base.InitializeDatabase(context);
            }

        protected override void Seed(CMDbContext context)
            {
            DataSeeder.Seed(context);
            base.Seed(context);
            }
        }

    class MigrateDatabaseToLatestVersion : MigrateDatabaseToLatestVersion<CMDbContext, Migrations.Configuration>
        {
        public override void InitializeDatabase(CMDbContext context)
            {
            base.InitializeDatabase(context);
            }
        }

    class NoInitializer : IDatabaseInitializer<CMDbContext>
        {

        public void InitializeDatabase(CMDbContext context)
            {

            }
        }

    class DataSeeder
        {

        public static void Seed(CMDbContext context)
            {
            //Initialize any Pre-requisite data
            }
        }
    }
