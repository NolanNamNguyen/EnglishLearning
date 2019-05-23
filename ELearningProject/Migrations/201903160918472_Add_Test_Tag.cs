namespace ELearningProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Test_Tag : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tests", "Tags", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tests", "Tags");
        }
    }
}
