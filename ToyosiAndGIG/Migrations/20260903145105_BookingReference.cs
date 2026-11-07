using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToyosiAndGIG.Migrations
{
    /// <inheritdoc />
    public partial class BookingReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BookingNumber",
                table: "Bookings",
                newName: "BookingReference");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BookingReference",
                table: "Bookings",
                newName: "BookingNumber");
        }
    }
}
