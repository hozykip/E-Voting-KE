namespace E_Vote_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoterProfile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tb_Voters", "ProfilePicture", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.tb_Voters", "ProfilePicture");
        }
    }
}
