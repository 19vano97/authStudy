using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityServer8.Data.Migrations.Operational
{
    /// <inheritdoc />
    public partial class MakePersistedGrantsDescriptionNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "PersistedGrants",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,                // <-- allow NULLs now
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // on rollback, make it non-nullable again (you may need a default)
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "PersistedGrants",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",              // ensure no NULLs on rollback
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);
        }
    }
}
