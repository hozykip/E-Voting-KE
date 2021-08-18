namespace E_Vote_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addtestfieldtousers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "TestField", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "TestField");
        }
    }
}
