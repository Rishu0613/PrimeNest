using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrimeNest.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class PropertyType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insert default property types
            migrationBuilder.InsertData(
                table: "PropertiesType",
                columns: new[] { "Name" },
                values: new object[,]
                {
            { "Residential" },
            { "Commercial" },
            { "Industrial" },
            { "Land" },
            { "Luxury" },
            { "Vacation" },
            { "Farm/Ranch" },
            { "Waterfront" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
