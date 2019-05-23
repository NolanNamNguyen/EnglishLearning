namespace ELearningProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserImage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Web_user", "UserImage", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Web_user", "UserImage");
        }
    }
}
