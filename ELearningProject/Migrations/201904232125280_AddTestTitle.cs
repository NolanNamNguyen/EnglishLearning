namespace ELearningProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTestTitle : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tests", "Title", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tests", "Title");
        }
    }
}
