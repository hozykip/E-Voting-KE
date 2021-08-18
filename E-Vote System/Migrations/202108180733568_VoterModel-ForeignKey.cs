namespace E_Vote_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoterModelForeignKey : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.tb_Voters", name: "VoterCategoryId", newName: "VoterCategoryModelId");
            RenameIndex(table: "dbo.tb_Voters", name: "IX_VoterCategoryId", newName: "IX_VoterCategoryModelId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.tb_Voters", name: "IX_VoterCategoryModelId", newName: "IX_VoterCategoryId");
            RenameColumn(table: "dbo.tb_Voters", name: "VoterCategoryModelId", newName: "VoterCategoryId");
        }
    }
}
