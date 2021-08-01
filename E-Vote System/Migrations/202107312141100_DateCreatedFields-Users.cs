namespace E_Vote_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DateCreatedFieldsUsers : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "DateCreated", c => c.DateTime(nullable: false));
            AlterColumn("dbo.AspNetUsers", "DateModified", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "DateModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.AspNetUsers", "DateCreated", c => c.DateTime(nullable: false));
        }
    }
}
