namespace ELearningProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTestImage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tests", "Image", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tests", "Image");
        }
    }
}
