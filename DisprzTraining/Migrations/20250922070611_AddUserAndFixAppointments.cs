using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DisprzTraining.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAndFixAppointments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Create Users table
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            // Step 2: Create unique index on Username
            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            // Step 3: Insert default user
            migrationBuilder.Sql(@"
                INSERT INTO Users (Username, Password, Email, FullName, CreatedAt)
                VALUES ('default', 'default123', 'default@example.com', 'Default User', GETUTCDATE())
            ");

            // Step 4: Add UserId column to Appointments table
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 1);  // Default to user ID 1

            // Step 5: Create index on UserId
            migrationBuilder.CreateIndex(
                name: "IX_Appointments_UserId",
                table: "Appointments",
                column: "UserId");

            // Step 6: Add foreign key constraint
            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Users_UserId",
                table: "Appointments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Step 1: Drop foreign key constraint
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Users_UserId",
                table: "Appointments");

            // Step 2: Drop index
            migrationBuilder.DropIndex(
                name: "IX_Appointments_UserId",
                table: "Appointments");

            // Step 3: Drop UserId column
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Appointments");

            // Step 4: Drop Users table
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
