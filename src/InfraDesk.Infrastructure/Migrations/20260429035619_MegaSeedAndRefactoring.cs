using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace InfraDesk.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MegaSeedAndRefactoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AssetTypes_Name",
                table: "AssetTypes");

            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                table: "Assets",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "Assets",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Assets",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true),
                    RoomNumber = table.Column<string>(type: "text", nullable: true),
                    ParentLocationId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locations_Locations_ParentLocationId",
                        column: x => x.ParentLocationId,
                        principalTable: "Locations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Softwares",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<string>(type: "text", nullable: true),
                    ManufacturerId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Softwares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Softwares_Manufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Domain = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Licenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    LicenseKey = table.Column<string>(type: "text", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    SoftwareId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Licenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Licenses_Softwares_SoftwareId",
                        column: x => x.SoftwareId,
                        principalTable: "Softwares",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Department = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    LeadId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teams_Persons_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Priority = table.Column<string>(type: "text", nullable: false),
                    RequesterId = table.Column<Guid>(type: "uuid", nullable: true),
                    AssignedAssetId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_Assets_AssignedAssetId",
                        column: x => x.AssignedAssetId,
                        principalTable: "Assets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tickets_Persons_RequesterId",
                        column: x => x.RequesterId,
                        principalTable: "Persons",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "AssetTypes",
                columns: new[] { "Id", "CreatedAt", "Description", "IconKey", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("d3689eb8-0421-471b-a988-c8ca3ff84269"), new DateTime(2026, 4, 29, 3, 56, 19, 594, DateTimeKind.Utc).AddTicks(648), null, "LaptopIcon", false, "Laptop", null },
                    { new Guid("d8cd223f-b1e1-4d9f-8054-336f53d86b6a"), new DateTime(2026, 4, 29, 3, 56, 19, 594, DateTimeKind.Utc).AddTicks(647), null, "ServerIcon", false, "Server", null }
                });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Id", "Address", "CreatedAt", "IsDeleted", "Name", "ParentLocationId", "RoomNumber", "TenantId", "UpdatedAt" },
                values: new object[] { new Guid("5ecf02f4-0985-447f-8049-b0e2609b7dd8"), "Musterstraße 1", new DateTime(2026, 4, 29, 3, 56, 19, 594, DateTimeKind.Utc).AddTicks(569), false, "Hauptsitz Berlin", null, null, new Guid("00000000-0000-0000-0000-000000000001"), null });

            migrationBuilder.InsertData(
                table: "Manufacturers",
                columns: new[] { "Id", "CreatedAt", "IsDeleted", "Name", "SupportContact", "UpdatedAt" },
                values: new object[] { new Guid("8306d64d-a981-4cd3-8828-a4d7ace57d56"), new DateTime(2026, 4, 29, 3, 56, 19, 594, DateTimeKind.Utc).AddTicks(632), false, "Dell Technologies", null, null });

            migrationBuilder.InsertData(
                table: "Teams",
                columns: new[] { "Id", "CreatedAt", "IsDeleted", "LeadId", "Name", "TenantId", "UpdatedAt" },
                values: new object[] { new Guid("6dc5d06e-6d71-4fe6-9bce-f439830d638b"), new DateTime(2026, 4, 29, 3, 56, 19, 594, DateTimeKind.Utc).AddTicks(594), false, null, "IT-Administration", new Guid("00000000-0000-0000-0000-000000000001"), null });

            migrationBuilder.InsertData(
                table: "Tenants",
                columns: new[] { "Id", "CreatedAt", "Domain", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2026, 4, 29, 3, 56, 19, 594, DateTimeKind.Utc).AddTicks(444), "musterfirma.de", false, "Musterfirma GmbH", null });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Id", "Address", "CreatedAt", "IsDeleted", "Name", "ParentLocationId", "RoomNumber", "TenantId", "UpdatedAt" },
                values: new object[] { new Guid("4c40e4fd-a669-4220-9d34-f7e2d4392fee"), null, new DateTime(2026, 4, 29, 3, 56, 19, 594, DateTimeKind.Utc).AddTicks(571), false, "Serverraum 01", new Guid("5ecf02f4-0985-447f-8049-b0e2609b7dd8"), "UG-01", new Guid("00000000-0000-0000-0000-000000000001"), null });

            migrationBuilder.InsertData(
                table: "Persons",
                columns: new[] { "Id", "CreatedAt", "Department", "Email", "FirstName", "IsDeleted", "LastName", "TeamId", "TenantId", "UpdatedAt" },
                values: new object[] { new Guid("027b9676-b1e9-42e2-b9d3-e315fea4e62f"), new DateTime(2026, 4, 29, 3, 56, 19, 594, DateTimeKind.Utc).AddTicks(613), null, "m.admin@musterfirma.de", "Max", false, "Admin", new Guid("6dc5d06e-6d71-4fe6-9bce-f439830d638b"), new Guid("00000000-0000-0000-0000-000000000001"), null });

            migrationBuilder.InsertData(
                table: "Assets",
                columns: new[] { "Id", "AssetTypeId", "CreatedAt", "DynamicDataJson", "InventoryNumber", "IsDeleted", "LocationId", "ManufacturerId", "Name", "OwnerId", "SerialNumber", "TenantId", "UpdatedAt" },
                values: new object[] { new Guid("313f9b02-c081-47aa-84bc-74bc17b14782"), new Guid("d8cd223f-b1e1-4d9f-8054-336f53d86b6a"), new DateTime(2026, 4, 29, 3, 56, 19, 594, DateTimeKind.Utc).AddTicks(663), "{\"CPU\": \"32 Cores\", \"RAM\": \"128GB\", \"OS\": \"Windows Server 2022\"}", "INV-0001", false, new Guid("4c40e4fd-a669-4220-9d34-f7e2d4392fee"), new Guid("8306d64d-a981-4cd3-8828-a4d7ace57d56"), "SRV-DC-01", new Guid("027b9676-b1e9-42e2-b9d3-e315fea4e62f"), "DELL-12345", new Guid("00000000-0000-0000-0000-000000000001"), null });

            migrationBuilder.InsertData(
                table: "Tickets",
                columns: new[] { "Id", "AssignedAssetId", "CreatedAt", "Description", "IsDeleted", "Priority", "RequesterId", "Status", "TenantId", "Title", "UpdatedAt" },
                values: new object[] { new Guid("a5f061b1-e6b4-429a-a7e4-c63be414297a"), new Guid("313f9b02-c081-47aa-84bc-74bc17b14782"), new DateTime(2026, 4, 29, 3, 56, 19, 594, DateTimeKind.Utc).AddTicks(682), "Der Server meldet einen defekten Sektor auf Disk 0.", false, "High", new Guid("027b9676-b1e9-42e2-b9d3-e315fea4e62f"), "Open", new Guid("00000000-0000-0000-0000-000000000001"), "Festplattentausch erforderlich", null });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_LocationId",
                table: "Assets",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_OwnerId",
                table: "Assets",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Licenses_SoftwareId",
                table: "Licenses",
                column: "SoftwareId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_ParentLocationId",
                table: "Locations",
                column: "ParentLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_TeamId",
                table: "Persons",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Softwares_ManufacturerId",
                table: "Softwares",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_LeadId",
                table: "Teams",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_AssignedAssetId",
                table: "Tickets",
                column: "AssignedAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_RequesterId",
                table: "Tickets",
                column: "RequesterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Locations_LocationId",
                table: "Assets",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Persons_OwnerId",
                table: "Assets",
                column: "OwnerId",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Persons_Teams_TeamId",
                table: "Persons",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Locations_LocationId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Persons_OwnerId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_Persons_Teams_TeamId",
                table: "Persons");

            migrationBuilder.DropTable(
                name: "Licenses");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "Softwares");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropIndex(
                name: "IX_Assets_LocationId",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_OwnerId",
                table: "Assets");

            migrationBuilder.DeleteData(
                table: "AssetTypes",
                keyColumn: "Id",
                keyValue: new Guid("d3689eb8-0421-471b-a988-c8ca3ff84269"));

            migrationBuilder.DeleteData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: new Guid("313f9b02-c081-47aa-84bc-74bc17b14782"));

            migrationBuilder.DeleteData(
                table: "AssetTypes",
                keyColumn: "Id",
                keyValue: new Guid("d8cd223f-b1e1-4d9f-8054-336f53d86b6a"));

            migrationBuilder.DeleteData(
                table: "Manufacturers",
                keyColumn: "Id",
                keyValue: new Guid("8306d64d-a981-4cd3-8828-a4d7ace57d56"));

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Assets");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTypes_Name",
                table: "AssetTypes",
                column: "Name",
                unique: true);
        }
    }
}
