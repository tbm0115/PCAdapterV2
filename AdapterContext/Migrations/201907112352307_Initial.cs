namespace AdapterContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "mtc.Adapters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Created = c.DateTime(nullable: false),
                        LastStarted = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "mtc.DataItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        TickFrequency = c.Int(nullable: false),
                        AdapterConfig_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("mtc.Adapters", t => t.AdapterConfig_Id)
                .Index(t => t.AdapterConfig_Id);
            
            CreateTable(
                "mtc.Durations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Started = c.DateTime(nullable: false),
                        Ended = c.DateTime(nullable: false),
                        Timespan = c.Long(nullable: false),
                        Item_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("mtc.DataItems", t => t.Item_Id)
                .Index(t => t.Item_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("mtc.Durations", "Item_Id", "mtc.DataItems");
            DropForeignKey("mtc.DataItems", "AdapterConfig_Id", "mtc.Adapters");
            DropIndex("mtc.Durations", new[] { "Item_Id" });
            DropIndex("mtc.DataItems", new[] { "AdapterConfig_Id" });
            DropTable("mtc.Durations");
            DropTable("mtc.DataItems");
            DropTable("mtc.Adapters");
        }
    }
}
