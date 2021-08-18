namespace E_Vote_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class notificationModel_dates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tb_Notifications", "StartDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.tb_Notifications", "EndDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.tb_Notifications", "EndDate");
            DropColumn("dbo.tb_Notifications", "StartDate");
        }
    }
}
