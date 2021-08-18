namespace E_Vote_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoterCategories : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tb_VoterCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.tb_Elections", "VoterCategoryId", c => c.Int(nullable: false));
            AddColumn("dbo.tb_Voters", "VoterCategoryId", c => c.Int(nullable: false));
            CreateIndex("dbo.tb_Elections", "VoterCategoryId");
            CreateIndex("dbo.tb_Voters", "VoterCategoryId");
            AddForeignKey("dbo.tb_Elections", "VoterCategoryId", "dbo.tb_VoterCategories", "Id", cascadeDelete: true);
            AddForeignKey("dbo.tb_Voters", "VoterCategoryId", "dbo.tb_VoterCategories", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tb_Voters", "VoterCategoryId", "dbo.tb_VoterCategories");
            DropForeignKey("dbo.tb_Elections", "VoterCategoryId", "dbo.tb_VoterCategories");
            DropIndex("dbo.tb_Voters", new[] { "VoterCategoryId" });
            DropIndex("dbo.tb_Elections", new[] { "VoterCategoryId" });
            DropColumn("dbo.tb_Voters", "VoterCategoryId");
            DropColumn("dbo.tb_Elections", "VoterCategoryId");
            DropTable("dbo.tb_VoterCategories");
        }
    }
}
