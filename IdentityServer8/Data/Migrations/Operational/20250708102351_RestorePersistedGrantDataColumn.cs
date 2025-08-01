using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityServer8.Data.Migrations.Operational
{
    /// <inheritdoc />
    public partial class RestorePersistedGrantDataColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
{
    // **Remove any AddColumn("Data",…) here** and replace it with:
    migrationBuilder.AlterColumn<string>(
        name: "Data",
        table: "PersistedGrants",
        type: "nvarchar(max)",
        maxLength: 50000,
        nullable: false,            // keep it non-nullable
        oldClrType: typeof(string),
        oldType: "nvarchar(max)",
        oldMaxLength: 50000,
        oldNullable: true);
}

protected override void Down(MigrationBuilder migrationBuilder)
{
    // **Remove any DropColumn("Data",…) here** and replace it with:
    migrationBuilder.AlterColumn<string>(
        name: "Data",
        table: "PersistedGrants",
        type: "nvarchar(max)",
        maxLength: 50000,
        nullable: true,             // roll back to nullable
        oldClrType: typeof(string),
        oldType: "nvarchar(max)",
        oldMaxLength: 50000);
}
    }
}
