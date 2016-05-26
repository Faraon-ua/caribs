namespace Caribs.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddEmailAndLastAwardedOnMlmAccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MlmAccounts", "Email", c => c.String(nullable: false, defaultValue: "dummy@mail.com"));
            AddColumn("dbo.MlmAccounts", "LastAwardedOn", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.MlmAccounts", "LastAwardedOn");
            DropColumn("dbo.MlmAccounts", "Email");
        }
    }
}
