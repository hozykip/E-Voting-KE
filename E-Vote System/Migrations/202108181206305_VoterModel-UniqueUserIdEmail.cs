namespace E_Vote_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoterModelUniqueUserIdEmail : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.tb_Voters", new[] { "UserId" });
            CreateIndex("dbo.tb_Voters", "EmailAddress", unique: true);
            CreateIndex("dbo.tb_Voters", "UserId", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.tb_Voters", new[] { "UserId" });
            DropIndex("dbo.tb_Voters", new[] { "EmailAddress" });
            CreateIndex("dbo.tb_Voters", "UserId");
        }
    }
}
