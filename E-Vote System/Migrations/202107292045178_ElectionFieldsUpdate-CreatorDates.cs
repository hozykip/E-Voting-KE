namespace E_Vote_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ElectionFieldsUpdateCreatorDates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tb_Elections", "CreatedBy", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.tb_Elections", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.tb_Elections", "DateModified", c => c.DateTime(nullable: false));
            CreateIndex("dbo.tb_Elections", "CreatedBy");
            AddForeignKey("dbo.tb_Elections", "CreatedBy", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tb_Elections", "CreatedBy", "dbo.AspNetUsers");
            DropIndex("dbo.tb_Elections", new[] { "CreatedBy" });
            DropColumn("dbo.tb_Elections", "DateModified");
            DropColumn("dbo.tb_Elections", "DateCreated");
            DropColumn("dbo.tb_Elections", "CreatedBy");
        }
    }
}
