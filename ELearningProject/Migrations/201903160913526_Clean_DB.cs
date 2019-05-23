namespace ELearningProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Clean_DB : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Questions", "Answer_id", "dbo.Answers");
            DropForeignKey("dbo.Questions", "Content_id", "dbo.QContents");
            DropForeignKey("dbo.Teachers", "User_id", "dbo.Web_user");
            DropForeignKey("dbo.Questions", "Type_id", "dbo.QTypes");
            DropIndex("dbo.Questions", new[] { "Answer_id" });
            DropIndex("dbo.Questions", new[] { "Content_id" });
            DropIndex("dbo.Questions", new[] { "Type_id" });
            DropIndex("dbo.TestQuestionDeploys", new[] { "Question_id" });
            DropIndex("dbo.TestQuestionDeploys", new[] { "Test_id" });
            DropIndex("dbo.Tests", new[] { "Creator_id" });
            DropIndex("dbo.Teachers", new[] { "User_id" });
            DropTable("dbo.Answers");
            DropTable("dbo.QContents");
            DropTable("dbo.QTypes");
            DropTable("dbo.Questions");
            DropTable("dbo.Teachers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Teachers",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        MeanRating = c.Single(nullable: false),
                        User_id = c.Int(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Tests",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Rating = c.Single(nullable: false),
                        Desc = c.String(),
                        Creator_id = c.Int(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.TestQuestionDeploys",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Order = c.Int(nullable: false),
                        Question_id = c.Int(),
                        Test_id = c.Int(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Level = c.Single(nullable: false),
                        Answer_id = c.Int(),
                        Content_id = c.Int(),
                        Type_id = c.Int(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.QTypes",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Desc = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.QContents",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Content = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Answers",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Content = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateIndex("dbo.Teachers", "User_id");
            CreateIndex("dbo.Tests", "Creator_id");
            CreateIndex("dbo.TestQuestionDeploys", "Test_id");
            CreateIndex("dbo.TestQuestionDeploys", "Question_id");
            CreateIndex("dbo.Questions", "Type_id");
            CreateIndex("dbo.Questions", "Content_id");
            CreateIndex("dbo.Questions", "Answer_id");
            AddForeignKey("dbo.Questions", "Type_id", "dbo.QTypes", "id");
            AddForeignKey("dbo.TestQuestionDeploys", "Test_id", "dbo.Tests", "id");
            AddForeignKey("dbo.Tests", "Creator_id", "dbo.Teachers", "id");
            AddForeignKey("dbo.Teachers", "User_id", "dbo.Web_user", "id");
            AddForeignKey("dbo.TestQuestionDeploys", "Question_id", "dbo.Questions", "id");
            AddForeignKey("dbo.Questions", "Content_id", "dbo.QContents", "id");
            AddForeignKey("dbo.Questions", "Answer_id", "dbo.Answers", "id");
        }
    }
}
