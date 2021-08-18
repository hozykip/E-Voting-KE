namespace E_Vote_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class notificationModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tb_Notifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        Message = c.String(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tb_Notifications", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.tb_Notifications", new[] { "UserId" });
            DropTable("dbo.tb_Notifications");
        }
    }
}
