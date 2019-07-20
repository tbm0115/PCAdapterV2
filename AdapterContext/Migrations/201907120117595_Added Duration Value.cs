namespace AdapterContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedDurationValue : DbMigration
    {
        public override void Up()
        {
            AddColumn("mtc.Durations", "Value", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("mtc.Durations", "Value");
        }
    }
}
