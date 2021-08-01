namespace E_Vote_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Votes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tb_Votes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VoterId = c.String(nullable: false, maxLength: 128),
                        CandidateId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.tb_ElectionCandidates", t => t.CandidateId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.VoterId, cascadeDelete: false)
                .Index(t => t.VoterId)
                .Index(t => t.CandidateId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tb_Votes", "VoterId", "dbo.AspNetUsers");
            DropForeignKey("dbo.tb_Votes", "CandidateId", "dbo.tb_ElectionCandidates");
            DropIndex("dbo.tb_Votes", new[] { "CandidateId" });
            DropIndex("dbo.tb_Votes", new[] { "VoterId" });
            DropTable("dbo.tb_Votes");
        }
    }
}
