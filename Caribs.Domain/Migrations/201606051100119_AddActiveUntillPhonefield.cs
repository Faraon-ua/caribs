namespace Caribs.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddActiveUntillPhonefield : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MlmAccounts", "Phone", c => c.String(maxLength: 16));
            AddColumn("dbo.MlmAccounts", "ActiveUntill", c => c.DateTime(nullable: false, storeType: "datetime2", defaultValue: DateTime.Now.AddYears(1)));
        }

        public override void Down()
        {
            DropColumn("dbo.MlmAccounts", "ActiveUntill");
            DropColumn("dbo.MlmAccounts", "Phone");
        }
    }
}
