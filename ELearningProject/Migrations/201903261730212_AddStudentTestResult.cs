namespace ELearningProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStudentTestResult : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TTypes",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            AddColumn("dbo.Questions", "Creator_id", c => c.Int());
            AddColumn("dbo.Tests", "Type_id", c => c.Int());
            CreateIndex("dbo.Questions", "Creator_id");
            CreateIndex("dbo.Tests", "Type_id");
            AddForeignKey("dbo.Questions", "Creator_id", "dbo.Teachers", "id");
            AddForeignKey("dbo.Tests", "Type_id", "dbo.TTypes", "id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tests", "Type_id", "dbo.TTypes");
            DropForeignKey("dbo.Questions", "Creator_id", "dbo.Teachers");
            DropIndex("dbo.Tests", new[] { "Type_id" });
            DropIndex("dbo.Questions", new[] { "Creator_id" });
            DropColumn("dbo.Tests", "Type_id");
            DropColumn("dbo.Questions", "Creator_id");
            DropTable("dbo.TTypes");
        }
    }
}
