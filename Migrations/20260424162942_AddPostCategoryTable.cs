using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InkWell.Category.Migrations
{
    /// <inheritdoc />
    public partial class AddPostCategoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PostCategories",
                columns: table => new
                {
                    PostCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostCategories", x => x.PostCategoryId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostCategories_PostId_CategoryId",
                table: "PostCategories",
                columns: new[] { "PostId", "CategoryId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostCategories");
        }
    }
}
