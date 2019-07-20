namespace AdapterContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetnullableDurationproperties : DbMigration
    {
        public override void Up()
        {
            AlterColumn("mtc.Durations", "Ended", c => c.DateTime());
            AlterColumn("mtc.Durations", "Timespan", c => c.Long());
        }
        
        public override void Down()
        {
            AlterColumn("mtc.Durations", "Timespan", c => c.Long(nullable: false));
            AlterColumn("mtc.Durations", "Ended", c => c.DateTime(nullable: false));
        }
    }
}
