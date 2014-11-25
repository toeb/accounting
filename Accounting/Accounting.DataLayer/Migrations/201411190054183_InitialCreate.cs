namespace Accounting.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Metas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreationDate = c.DateTime(nullable: false),
                        LastModified = c.DateTime(nullable: false),
                        Name = c.String(),
                        Name1 = c.String(),
                        Number = c.String(),
                        ShortName = c.String(),
                        Amount = c.Decimal(precision: 18, scale: 2),
                        Type = c.Int(),
                        ReceiptNumber = c.String(),
                        ReceiptDate = c.DateTime(),
                        Text = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        AccountCategory_Id = c.Int(),
                        Parent_Id = c.Int(),
                        Account_Id = c.Int(),
                        Transaction_Id = c.Int(),
                        Storno_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Metas", t => t.AccountCategory_Id)
                .ForeignKey("dbo.Metas", t => t.Parent_Id)
                .ForeignKey("dbo.Metas", t => t.Account_Id)
                .ForeignKey("dbo.Metas", t => t.Transaction_Id)
                .ForeignKey("dbo.Metas", t => t.Storno_Id)
                .Index(t => t.AccountCategory_Id)
                .Index(t => t.Parent_Id)
                .Index(t => t.Account_Id)
                .Index(t => t.Transaction_Id)
                .Index(t => t.Storno_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Metas", "Storno_Id", "dbo.Metas");
            DropForeignKey("dbo.Metas", "Transaction_Id", "dbo.Metas");
            DropForeignKey("dbo.Metas", "Account_Id", "dbo.Metas");
            DropForeignKey("dbo.Metas", "Parent_Id", "dbo.Metas");
            DropForeignKey("dbo.Metas", "AccountCategory_Id", "dbo.Metas");
            DropIndex("dbo.Metas", new[] { "Storno_Id" });
            DropIndex("dbo.Metas", new[] { "Transaction_Id" });
            DropIndex("dbo.Metas", new[] { "Account_Id" });
            DropIndex("dbo.Metas", new[] { "Parent_Id" });
            DropIndex("dbo.Metas", new[] { "AccountCategory_Id" });
            DropTable("dbo.Metas");
        }
    }
}
