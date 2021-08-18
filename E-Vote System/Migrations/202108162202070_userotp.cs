namespace E_Vote_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userotp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "OTP", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "OTP");
        }
    }
}
