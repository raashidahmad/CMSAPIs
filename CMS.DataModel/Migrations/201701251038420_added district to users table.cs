namespace CMS.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addeddistricttouserstable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EFComplaints", "District_Id", c => c.Int());
            AddColumn("dbo.AspNetUsers", "DistrictId", c => c.Int(nullable: false));
            CreateIndex("dbo.EFComplaints", "District_Id");
            AddForeignKey("dbo.EFComplaints", "District_Id", "dbo.EFDistricts", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EFComplaints", "District_Id", "dbo.EFDistricts");
            DropIndex("dbo.EFComplaints", new[] { "District_Id" });
            DropColumn("dbo.AspNetUsers", "DistrictId");
            DropColumn("dbo.EFComplaints", "District_Id");
        }
    }
}
