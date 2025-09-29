using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImgGen.Application.Migrations
{
    /// <inheritdoc />
    public partial class InitialImageStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "images");

            migrationBuilder.CreateTable(
                name: "image_data",
                schema: "images",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_image_data", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "image_metadata",
                schema: "images",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    image_data_id = table.Column<Guid>(type: "uuid", nullable: false),
                    original_file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    mime_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    file_size_bytes = table.Column<long>(type: "bigint", nullable: false),
                    uploaded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_image_metadata", x => x.id);
                    table.ForeignKey(
                        name: "FK_image_metadata_image_data_image_data_id",
                        column: x => x.image_data_id,
                        principalSchema: "images",
                        principalTable: "image_data",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_image_metadata_image_data_id",
                schema: "images",
                table: "image_metadata",
                column: "image_data_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_image_metadata_mime_type",
                schema: "images",
                table: "image_metadata",
                column: "mime_type");

            migrationBuilder.CreateIndex(
                name: "ix_image_metadata_uploaded_at",
                schema: "images",
                table: "image_metadata",
                column: "uploaded_at");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "image_metadata",
                schema: "images");

            migrationBuilder.DropTable(
                name: "image_data",
                schema: "images");
        }
    }
}
