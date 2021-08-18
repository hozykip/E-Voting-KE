namespace E_Vote_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class notificationModel_title : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tb_Notifications", "Title", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.tb_Notifications", "Title");
        }
    }
}
