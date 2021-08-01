namespace E_Vote_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatedAtDefaultValue : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.AspNetUsers", "DateModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_ElectionCandidates", "DateCreated", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_ElectionPositions", "DateCreated", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_Elections", "DateCreated", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_Votes", "DateCreated", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_ElectionTypes", "DateCreated", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.tb_ElectionTypes", "DateCreated", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_Votes", "DateCreated", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_Elections", "DateCreated", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_ElectionPositions", "DateCreated", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_ElectionCandidates", "DateCreated", c => c.DateTime(nullable: false));
            DropColumn("dbo.AspNetUsers", "DateModified");
            DropColumn("dbo.AspNetUsers", "DateCreated");
        }
    }
}
