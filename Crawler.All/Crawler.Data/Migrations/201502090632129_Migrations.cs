namespace Crawler.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migrations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Websites", "website_logo", c => c.String(maxLength: 250, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Websites", "website_logo");
        }
    }
}
