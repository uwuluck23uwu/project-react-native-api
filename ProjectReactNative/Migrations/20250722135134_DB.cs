using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectReactNative.Migrations
{
    /// <inheritdoc />
    public partial class DB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    event_id = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    location_coordinates = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    event_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    start_time = table.Column<DateTime>(type: "datetime", nullable: true),
                    end_time = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Events__2370F727EB0E454C", x => x.event_id);
                });

            migrationBuilder.CreateTable(
                name: "Facilities",
                columns: table => new
                {
                    facility_id = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    location_coordinates = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    opening_hours = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Faciliti__B2E8EAAE2B6B4015", x => x.facility_id);
                });

            migrationBuilder.CreateTable(
                name: "Habitats",
                columns: table => new
                {
                    habitat_id = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Habitats__3B715112D01A7D93", x => x.habitat_id);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    image_id = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ref_id = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    image_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    uploaded_date = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Images__DC9AC9556D4A6F01", x => x.image_id);
                });

            migrationBuilder.CreateTable(
                name: "News",
                columns: table => new
                {
                    news_id = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    contents = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    published_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__News__4C27CCD8FCA100F4", x => x.news_id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    product_id = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    price = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    stock_quantity = table.Column<int>(type: "int", nullable: true),
                    qr_code_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Products__47027DF54B09C236", x => x.product_id);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    refresh_token_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    jwt_token_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    token = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    is_valid = table.Column<bool>(type: "bit", nullable: true),
                    expires_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.refresh_token_id);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    ticket_id = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ticket_type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    price = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    purchase_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    visit_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Tickets__D596F96B18CDAE9B", x => x.ticket_id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    image_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    role = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__B9BE370F01B068F3", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "Animals",
                columns: table => new
                {
                    animal_id = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    habitat_id = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    species = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    scientific_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    location_coordinates = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    date_of_birth = table.Column<DateTime>(type: "datetime", nullable: true),
                    arrival_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Animals__DE680F92C5D76682", x => x.animal_id);
                    table.ForeignKey(
                        name: "FK__Animals__habitat__6EF57B66",
                        column: x => x.habitat_id,
                        principalTable: "Habitats",
                        principalColumn: "habitat_id");
                });

            migrationBuilder.CreateTable(
                name: "QR_Scan_Logs",
                columns: table => new
                {
                    scan_id = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    product_id = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    qr_code_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    scanned_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__QR_Scan___9846B9BB7F463C10", x => x.scan_id);
                    table.ForeignKey(
                        name: "FK__QR_Scan_L__produ__6A30C649",
                        column: x => x.product_id,
                        principalTable: "Products",
                        principalColumn: "product_id");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    order_id = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    user_id = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    total_amount = table.Column<int>(type: "int", nullable: true),
                    total_price = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    payment_status = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    order_datetime = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Orders__46596229C4B30A05", x => x.order_id);
                    table.ForeignKey(
                        name: "FK__Orders__user_id__6383C8BA",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Staffs",
                columns: table => new
                {
                    staff_id = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    user_id = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    first_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    last_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    position = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    hire_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Staff__1963DD9CEED18550", x => x.staff_id);
                    table.ForeignKey(
                        name: "FK__Staff__user_id__5BE2A6F2",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "User_Ticket",
                columns: table => new
                {
                    user_ticket_id = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    user_id = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    ticket_id = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    assigned_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__User_Tic__6E36A6B60B25ECAC", x => x.user_ticket_id);
                    table.ForeignKey(
                        name: "FK__User_Tick__ticke__59063A47",
                        column: x => x.ticket_id,
                        principalTable: "Tickets",
                        principalColumn: "ticket_id");
                    table.ForeignKey(
                        name: "FK__User_Tick__user___5812160E",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Order_Items",
                columns: table => new
                {
                    order_item_id = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    order_id = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    product_id = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: true),
                    price_each = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Order_It__3764B6BC10BA2CFE", x => x.order_item_id);
                    table.ForeignKey(
                        name: "FK__Order_Ite__order__66603565",
                        column: x => x.order_id,
                        principalTable: "Orders",
                        principalColumn: "order_id");
                    table.ForeignKey(
                        name: "FK__Order_Ite__produ__6754599E",
                        column: x => x.product_id,
                        principalTable: "Products",
                        principalColumn: "product_id");
                });

            migrationBuilder.CreateTable(
                name: "EventStaffs",
                columns: table => new
                {
                    event_staff_id = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    staff_id = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    event_id = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    role_in_event = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__EventSta__3E47C255B5E6D4C2", x => x.event_staff_id);
                    table.ForeignKey(
                        name: "FK__EventStaf__event__797309D9",
                        column: x => x.event_id,
                        principalTable: "Events",
                        principalColumn: "event_id");
                    table.ForeignKey(
                        name: "FK__EventStaf__staff__787EE5A0",
                        column: x => x.staff_id,
                        principalTable: "Staffs",
                        principalColumn: "staff_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Animals_habitat_id",
                table: "Animals",
                column: "habitat_id");

            migrationBuilder.CreateIndex(
                name: "IX_EventStaffs_event_id",
                table: "EventStaffs",
                column: "event_id");

            migrationBuilder.CreateIndex(
                name: "IX_EventStaffs_staff_id",
                table: "EventStaffs",
                column: "staff_id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_Items_order_id",
                table: "Order_Items",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_Items_product_id",
                table: "Order_Items",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_user_id",
                table: "Orders",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_QR_Scan_Logs_product_id",
                table: "QR_Scan_Logs",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_user_id",
                table: "Staffs",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_User_Ticket_ticket_id",
                table: "User_Ticket",
                column: "ticket_id");

            migrationBuilder.CreateIndex(
                name: "IX_User_Ticket_user_id",
                table: "User_Ticket",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Animals");

            migrationBuilder.DropTable(
                name: "EventStaffs");

            migrationBuilder.DropTable(
                name: "Facilities");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "News");

            migrationBuilder.DropTable(
                name: "Order_Items");

            migrationBuilder.DropTable(
                name: "QR_Scan_Logs");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "User_Ticket");

            migrationBuilder.DropTable(
                name: "Habitats");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Staffs");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
