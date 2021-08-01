namespace E_Vote_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NullableDateModifiedFields : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.tb_ElectionCandidates", "DateModified", c => c.DateTime());
            AlterColumn("dbo.tb_ElectionPositions", "DateModified", c => c.DateTime());
            AlterColumn("dbo.tb_Elections", "DateModified", c => c.DateTime());
            AlterColumn("dbo.tb_ElectionTypes", "DateModified", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.tb_ElectionTypes", "DateModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_Elections", "DateModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_ElectionPositions", "DateModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_ElectionCandidates", "DateModified", c => c.DateTime(nullable: false));
        }
    }
}
