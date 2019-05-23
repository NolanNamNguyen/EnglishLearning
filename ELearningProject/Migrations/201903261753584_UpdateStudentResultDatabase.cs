namespace ELearningProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateStudentResultDatabase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StudentTestResults",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Score = c.Single(nullable: false),
                        Student_id = c.Int(),
                        Test_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Students", t => t.Student_id)
                .ForeignKey("dbo.Tests", t => t.Test_id)
                .Index(t => t.Student_id)
                .Index(t => t.Test_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StudentTestResults", "Test_id", "dbo.Tests");
            DropForeignKey("dbo.StudentTestResults", "Student_id", "dbo.Students");
            DropIndex("dbo.StudentTestResults", new[] { "Test_id" });
            DropIndex("dbo.StudentTestResults", new[] { "Student_id" });
            DropTable("dbo.StudentTestResults");
        }
    }
}
