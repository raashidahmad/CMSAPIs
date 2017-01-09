namespace CMS.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial_Migration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EFCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EFComplainants",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FullName = c.String(),
                        NIC = c.String(),
                        Address = c.String(),
                        Mobile = c.String(),
                        ContactMedium = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EFComplaints",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Dated = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                        Category_Id = c.Int(),
                        Complainant_Id = c.Int(nullable: false),
                        SDC_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EFCategories", t => t.Category_Id)
                .ForeignKey("dbo.EFComplainants", t => t.Complainant_Id, cascadeDelete: true)
                .ForeignKey("dbo.EFSDCs", t => t.SDC_Id)
                .Index(t => t.Category_Id)
                .Index(t => t.Complainant_Id)
                .Index(t => t.SDC_Id);
            
            CreateTable(
                "dbo.EFDocuments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Path = c.String(nullable: false),
                        Complaint_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EFComplaints", t => t.Complaint_Id, cascadeDelete: true)
                .Index(t => t.Complaint_Id);
            
            CreateTable(
                "dbo.EFPhoneCallLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Dated = c.DateTime(nullable: false),
                        Complain_Id = c.Int(),
                        Complainant_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EFComplaints", t => t.Complain_Id)
                .ForeignKey("dbo.EFComplainants", t => t.Complainant_Id)
                .Index(t => t.Complain_Id)
                .Index(t => t.Complainant_Id);
            
            CreateTable(
                "dbo.EFSDCs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        District_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EFDistricts", t => t.District_Id)
                .Index(t => t.District_Id);
            
            CreateTable(
                "dbo.EFDistricts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        SMSPrefix = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EFSMSLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Dated = c.DateTime(nullable: false),
                        Complain_Id = c.Int(),
                        Complainant_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EFComplaints", t => t.Complain_Id)
                .ForeignKey("dbo.EFComplainants", t => t.Complainant_Id)
                .Index(t => t.Complain_Id)
                .Index(t => t.Complainant_Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        PublicUserId = c.String(nullable: false),
                        FirstName = c.String(nullable: false, maxLength: 100),
                        LastName = c.String(nullable: false, maxLength: 100),
                        SDCId = c.Int(nullable: false),
                        Mobile = c.String(),
                        CreationDate = c.DateTime(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.EFSMSLogs", "Complainant_Id", "dbo.EFComplainants");
            DropForeignKey("dbo.EFSMSLogs", "Complain_Id", "dbo.EFComplaints");
            DropForeignKey("dbo.EFComplaints", "SDC_Id", "dbo.EFSDCs");
            DropForeignKey("dbo.EFSDCs", "District_Id", "dbo.EFDistricts");
            DropForeignKey("dbo.EFPhoneCallLogs", "Complainant_Id", "dbo.EFComplainants");
            DropForeignKey("dbo.EFPhoneCallLogs", "Complain_Id", "dbo.EFComplaints");
            DropForeignKey("dbo.EFDocuments", "Complaint_Id", "dbo.EFComplaints");
            DropForeignKey("dbo.EFComplaints", "Complainant_Id", "dbo.EFComplainants");
            DropForeignKey("dbo.EFComplaints", "Category_Id", "dbo.EFCategories");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.EFSMSLogs", new[] { "Complainant_Id" });
            DropIndex("dbo.EFSMSLogs", new[] { "Complain_Id" });
            DropIndex("dbo.EFSDCs", new[] { "District_Id" });
            DropIndex("dbo.EFPhoneCallLogs", new[] { "Complainant_Id" });
            DropIndex("dbo.EFPhoneCallLogs", new[] { "Complain_Id" });
            DropIndex("dbo.EFDocuments", new[] { "Complaint_Id" });
            DropIndex("dbo.EFComplaints", new[] { "SDC_Id" });
            DropIndex("dbo.EFComplaints", new[] { "Complainant_Id" });
            DropIndex("dbo.EFComplaints", new[] { "Category_Id" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.EFSMSLogs");
            DropTable("dbo.EFDistricts");
            DropTable("dbo.EFSDCs");
            DropTable("dbo.EFPhoneCallLogs");
            DropTable("dbo.EFDocuments");
            DropTable("dbo.EFComplaints");
            DropTable("dbo.EFComplainants");
            DropTable("dbo.EFCategories");
        }
    }
}
