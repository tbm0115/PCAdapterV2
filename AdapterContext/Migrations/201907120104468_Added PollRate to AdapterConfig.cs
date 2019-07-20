namespace AdapterContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPollRatetoAdapterConfig : DbMigration
    {
        public override void Up()
        {
            AddColumn("mtc.Adapters", "PollRate", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("mtc.Adapters", "PollRate");
        }
    }
}
