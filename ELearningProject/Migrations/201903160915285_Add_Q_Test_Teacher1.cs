namespace ELearningProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Q_Test_Teacher1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Answers",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Content = c.String(),
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
                "dbo.QTypes",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Desc = c.String(),
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
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Answers", t => t.Answer_id)
                .ForeignKey("dbo.QContents", t => t.Content_id)
                .ForeignKey("dbo.QTypes", t => t.Type_id)
                .Index(t => t.Answer_id)
                .Index(t => t.Content_id)
                .Index(t => t.Type_id);
            
            CreateTable(
                "dbo.TestQuestionDeploys",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Order = c.Int(nullable: false),
                        Question_id = c.Int(),
                        Test_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Questions", t => t.Question_id)
                .ForeignKey("dbo.Tests", t => t.Test_id)
                .Index(t => t.Question_id)
                .Index(t => t.Test_id);
            
            CreateTable(
                "dbo.Tests",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Rating = c.Single(nullable: false),
                        Desc = c.String(),
                        Creator_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Teachers", t => t.Creator_id)
                .Index(t => t.Creator_id);
            
            CreateTable(
                "dbo.Teachers",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        MeanRating = c.Single(nullable: false),
                        User_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Web_user", t => t.User_id)
                .Index(t => t.User_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Questions", "Type_id", "dbo.QTypes");
            DropForeignKey("dbo.TestQuestionDeploys", "Test_id", "dbo.Tests");
            DropForeignKey("dbo.Tests", "Creator_id", "dbo.Teachers");
            DropForeignKey("dbo.Teachers", "User_id", "dbo.Web_user");
            DropForeignKey("dbo.TestQuestionDeploys", "Question_id", "dbo.Questions");
            DropForeignKey("dbo.Questions", "Content_id", "dbo.QContents");
            DropForeignKey("dbo.Questions", "Answer_id", "dbo.Answers");
            DropIndex("dbo.Teachers", new[] { "User_id" });
            DropIndex("dbo.Tests", new[] { "Creator_id" });
            DropIndex("dbo.TestQuestionDeploys", new[] { "Test_id" });
            DropIndex("dbo.TestQuestionDeploys", new[] { "Question_id" });
            DropIndex("dbo.Questions", new[] { "Type_id" });
            DropIndex("dbo.Questions", new[] { "Content_id" });
            DropIndex("dbo.Questions", new[] { "Answer_id" });
            DropTable("dbo.Teachers");
            DropTable("dbo.Tests");
            DropTable("dbo.TestQuestionDeploys");
            DropTable("dbo.Questions");
            DropTable("dbo.QTypes");
            DropTable("dbo.QContents");
            DropTable("dbo.Answers");
        }
    }
}
