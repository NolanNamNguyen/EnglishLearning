namespace ELearningProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLocked : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Web_user", "Locked", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Web_user", "Locked");
        }
    }
}
