namespace Accounting.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class asd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Metas", "IsActive", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Metas", "IsActive");
        }
    }
}
