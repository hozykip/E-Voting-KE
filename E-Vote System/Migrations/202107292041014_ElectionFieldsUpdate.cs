namespace E_Vote_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ElectionFieldsUpdate : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.tb_Elections", "Type_Id", "dbo.tb_ElectionTypes");
            DropIndex("dbo.tb_Elections", new[] { "Type_Id" });
            DropColumn("dbo.tb_Elections", "TypeId");
            RenameColumn(table: "dbo.tb_Elections", name: "Type_Id", newName: "TypeId");
            AlterColumn("dbo.tb_Elections", "TypeId", c => c.Int(nullable: false));
            AlterColumn("dbo.tb_Elections", "TypeId", c => c.Int(nullable: false));
            CreateIndex("dbo.tb_Elections", "TypeId");
            AddForeignKey("dbo.tb_Elections", "TypeId", "dbo.tb_ElectionTypes", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tb_Elections", "TypeId", "dbo.tb_ElectionTypes");
            DropIndex("dbo.tb_Elections", new[] { "TypeId" });
            AlterColumn("dbo.tb_Elections", "TypeId", c => c.Int());
            AlterColumn("dbo.tb_Elections", "TypeId", c => c.String(nullable: false));
            RenameColumn(table: "dbo.tb_Elections", name: "TypeId", newName: "Type_Id");
            AddColumn("dbo.tb_Elections", "TypeId", c => c.String(nullable: false));
            CreateIndex("dbo.tb_Elections", "Type_Id");
            AddForeignKey("dbo.tb_Elections", "Type_Id", "dbo.tb_ElectionTypes", "Id");
        }
    }
}
