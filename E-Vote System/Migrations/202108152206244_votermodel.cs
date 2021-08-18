namespace E_Vote_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class votermodel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tb_Voters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        EmailAddress = c.String(nullable: false),
                        IdNumber = c.String(nullable: false),
                        Address = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tb_Voters", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.tb_Voters", new[] { "UserId" });
            DropTable("dbo.tb_Voters");
        }
    }
}
