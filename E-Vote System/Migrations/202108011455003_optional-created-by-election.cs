namespace E_Vote_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class optionalcreatedbyelection : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.tb_Elections", "CreatedBy", "dbo.AspNetUsers");
            DropIndex("dbo.tb_Elections", new[] { "CreatedBy" });
            AlterColumn("dbo.tb_Elections", "CreatedBy", c => c.String(maxLength: 128));
            CreateIndex("dbo.tb_Elections", "CreatedBy");
            AddForeignKey("dbo.tb_Elections", "CreatedBy", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tb_Elections", "CreatedBy", "dbo.AspNetUsers");
            DropIndex("dbo.tb_Elections", new[] { "CreatedBy" });
            AlterColumn("dbo.tb_Elections", "CreatedBy", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.tb_Elections", "CreatedBy");
            AddForeignKey("dbo.tb_Elections", "CreatedBy", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
