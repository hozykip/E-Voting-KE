namespace E_Vote_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ElectionTypesDates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tb_ElectionTypes", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.tb_ElectionTypes", "DateModified", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.tb_ElectionTypes", "DateModified");
            DropColumn("dbo.tb_ElectionTypes", "DateCreated");
        }
    }
}
