namespace E_Vote_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ElectionCandidates : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tb_ElectionCandidates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PositionId = c.Int(nullable: false),
                        SurName = c.String(nullable: false),
                        FirstName = c.String(nullable: false),
                        OtherName = c.String(),
                        EmailAddress = c.String(),
                        PhoneNumber = c.String(nullable: false),
                        ManifestoFile = c.String(),
                        ProfilePicture = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.tb_ElectionPositions", t => t.PositionId, cascadeDelete: true)
                .Index(t => t.PositionId);
            
            AlterColumn("dbo.tb_Elections", "StartDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_Elections", "EndDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tb_ElectionCandidates", "PositionId", "dbo.tb_ElectionPositions");
            DropIndex("dbo.tb_ElectionCandidates", new[] { "PositionId" });
            AlterColumn("dbo.tb_Elections", "EndDate", c => c.String(nullable: false));
            AlterColumn("dbo.tb_Elections", "StartDate", c => c.String(nullable: false));
            DropTable("dbo.tb_ElectionCandidates");
        }
    }
}
