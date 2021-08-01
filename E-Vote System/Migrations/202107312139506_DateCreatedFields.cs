namespace E_Vote_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DateCreatedFields : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.tb_ElectionCandidates", "DateCreated", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_ElectionPositions", "DateCreated", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_Elections", "DateCreated", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_Votes", "DateCreated", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_Votes", "DateModified", c => c.DateTime());
            AlterColumn("dbo.tb_ElectionTypes", "DateCreated", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.tb_ElectionTypes", "DateCreated", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_Votes", "DateModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_Votes", "DateCreated", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_Elections", "DateCreated", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_ElectionPositions", "DateCreated", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_ElectionCandidates", "DateCreated", c => c.DateTime(nullable: false));
        }
    }
}
