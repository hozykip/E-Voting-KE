namespace E_Vote_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ElectionPositions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tb_ElectionPositions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ElectionId = c.Int(nullable: false),
                        Position = c.String(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.tb_Elections", t => t.ElectionId, cascadeDelete: true)
                .Index(t => t.ElectionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tb_ElectionPositions", "ElectionId", "dbo.tb_Elections");
            DropIndex("dbo.tb_ElectionPositions", new[] { "ElectionId" });
            DropTable("dbo.tb_ElectionPositions");
        }
    }
}
