namespace E_Vote_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removetestfieldtousers : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "TestField");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "TestField", c => c.String());
        }
    }
}
