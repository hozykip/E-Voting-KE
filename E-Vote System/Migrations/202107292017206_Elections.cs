namespace E_Vote_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Elections : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tb_Elections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        StartDate = c.String(nullable: false),
                        EndDate = c.String(nullable: false),
                        TypeId = c.String(nullable: false),
                        Type_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.tb_ElectionTypes", t => t.Type_Id)
                .Index(t => t.Type_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tb_Elections", "Type_Id", "dbo.tb_ElectionTypes");
            DropIndex("dbo.tb_Elections", new[] { "Type_Id" });
            DropTable("dbo.tb_Elections");
        }
    }
}
