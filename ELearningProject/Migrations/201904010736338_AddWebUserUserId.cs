namespace ELearningProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddWebUserUserId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Web_user", "UserID", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Web_user", "UserID");
        }
    }
}
