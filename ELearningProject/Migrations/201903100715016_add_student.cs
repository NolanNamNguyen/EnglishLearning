namespace ELearningProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_student : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Users", newName: "Web_user");
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Skill_listening = c.Int(nullable: false),
                        Skill_speaking = c.Int(nullable: false),
                        Skill_reading = c.Int(nullable: false),
                        Skill_writing = c.Int(nullable: false),
                        Mean_skills = c.Int(nullable: false),
                        web_User_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Web_user", t => t.web_User_id)
                .Index(t => t.web_User_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Students", "web_User_id", "dbo.Web_user");
            DropIndex("dbo.Students", new[] { "web_User_id" });
            DropTable("dbo.Students");
            RenameTable(name: "dbo.Web_user", newName: "Users");
        }
    }
}
