using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Telegram_bot.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "catering_mode_operation_catering_id_catering_mode_operation_cat");

            migrationBuilder.CreateSequence(
                name: "catering_type_kitchen_id_catering_type_kitchen_seq");

            migrationBuilder.CreateSequence(
                name: "event_id_event_seq");

            migrationBuilder.CreateSequence(
                name: "feedback_id_feedback_seq");

            migrationBuilder.CreateSequence(
                name: "mode_operation_catering_id_mode_operation_catering_seq");

            migrationBuilder.CreateSequence(
                name: "operating_mode_id_operating_mode_seq");

            migrationBuilder.CreateSequence(
                name: "photo_sight_id_photo_sight_seq");

            migrationBuilder.CreateSequence(
                name: "route_catering_hotel_id_route_catering_hotel_seq");

            migrationBuilder.CreateSequence(
                name: "route_event_sight_id_route_event_sight_seq");

            migrationBuilder.CreateSequence(
                name: "route_id_route_seq");

            migrationBuilder.CreateSequence(
                name: "sight_id_sight_seq");

            migrationBuilder.CreateSequence(
                name: "sight_operating_mode_id_sight_operating_mode_seq");

            migrationBuilder.CreateSequence(
                name: "souvenir_id_souvenir_seq");

            migrationBuilder.CreateSequence(
                name: "special_day_catering_id_special_day_catering_seq");

            migrationBuilder.CreateSequence(
                name: "special_day_sight_id_special_day_sight_seq");

            migrationBuilder.CreateSequence(
                name: "ticket_id_ticket_seq");

            migrationBuilder.CreateSequence(
                name: "type_kitchen_id_type_kitchen_seq");

            migrationBuilder.CreateTable(
                name: "catering",
                columns: table => new
                {
                    id_catering = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    establishment_category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    establishment_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    establishment_street = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    establishment_house = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    establishment_phone = table.Column<string>(type: "character(11)", fixedLength: true, maxLength: 11, nullable: true),
                    catering_url = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("catering_pkey", x => x.id_catering);
                });

            migrationBuilder.CreateTable(
                name: "event_",
                columns: table => new
                {
                    id_event = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('event_id_event_seq'::regclass)"),
                    type_event = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name_event = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: false),
                    street_event = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    house_event = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    date_event = table.Column<DateOnly>(type: "date", nullable: true),
                    time_beginning_event = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    age_limit = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("event__pkey", x => x.id_event);
                });

            migrationBuilder.CreateTable(
                name: "hotel",
                columns: table => new
                {
                    id_hotel = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    hotel_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    hotel_street = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    hotel_house = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    contact_number_hotel = table.Column<string>(type: "character(11)", fixedLength: true, maxLength: 11, nullable: false),
                    hotel_url = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("hotel_pkey", x => x.id_hotel);
                });

            migrationBuilder.CreateTable(
                name: "mode_operation_catering",
                columns: table => new
                {
                    id_mode_operation_catering = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    working_day_week = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    beginning = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    end_day = table.Column<TimeOnly>(type: "time without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("mode_operation_catering_pkey", x => x.id_mode_operation_catering);
                });

            migrationBuilder.CreateTable(
                name: "operating_mode",
                columns: table => new
                {
                    id_operating_mode = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    start_time = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    end_time = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    day_of_week = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("operating_mode_pkey", x => x.id_operating_mode);
                });

            migrationBuilder.CreateTable(
                name: "sight",
                columns: table => new
                {
                    id_sight = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type_sight = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name_sight = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: false),
                    location_street = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    location_house = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    description = table.Column<string>(type: "character varying(900)", maxLength: 900, nullable: true),
                    number_seats = table.Column<int>(type: "integer", nullable: true),
                    contact_number = table.Column<string>(type: "character(11)", fixedLength: true, maxLength: 11, nullable: true),
                    email = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    sight_url = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("sight_pkey", x => x.id_sight);
                });

            migrationBuilder.CreateTable(
                name: "souvenir",
                columns: table => new
                {
                    id_souvenir = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    product = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name_souvenir = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    tastes = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    weight = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("souvenir_pkey", x => x.id_souvenir);
                });

            migrationBuilder.CreateTable(
                name: "special_day_catering",
                columns: table => new
                {
                    id_special_day_catering = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    date = table.Column<int>(type: "integer", nullable: true),
                    status_day = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    time_start_work = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    time_end_work = table.Column<TimeOnly>(type: "time without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("special_day_catering_pkey", x => x.id_special_day_catering);
                });

            migrationBuilder.CreateTable(
                name: "special_day_sight",
                columns: table => new
                {
                    id_special_day_sight = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    special_day_date = table.Column<int>(type: "integer", nullable: true),
                    special_day_status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    start_work = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    end_work = table.Column<TimeOnly>(type: "time without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("special_day_sight_pkey", x => x.id_special_day_sight);
                });

            migrationBuilder.CreateTable(
                name: "type_kitchen",
                columns: table => new
                {
                    id_type_kitchen = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name_type_kitchen = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("type_kitchen_pkey", x => x.id_type_kitchen);
                });

            migrationBuilder.CreateTable(
                name: "userbot",
                columns: table => new
                {
                    id_user = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    last_name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    name_person = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    patronymic = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    gender = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    phone_number = table.Column<string>(type: "character(11)", fixedLength: true, maxLength: 11, nullable: true),
                    date_birth = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("userbot_pkey", x => x.id_user);
                });

            migrationBuilder.CreateTable(
                name: "ticket",
                columns: table => new
                {
                    id_ticket = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    minimum_age = table.Column<int>(type: "integer", nullable: true),
                    maximum_age = table.Column<int>(type: "integer", nullable: true),
                    price = table.Column<decimal>(type: "money", nullable: true),
                    id_event = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ticket_pkey", x => x.id_ticket);
                    table.ForeignKey(
                        name: "fk_ticket_event",
                        column: x => x.id_event,
                        principalTable: "event_",
                        principalColumn: "id_event");
                });

            migrationBuilder.CreateTable(
                name: "catering_mode_operation_catering",
                columns: table => new
                {
                    id_catering_mode_operation_catering = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('catering_mode_operation_catering_id_catering_mode_operation_cat'::regclass)"),
                    id_mode_operation_catering = table.Column<int>(type: "integer", nullable: false),
                    id_catering = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("catering_mode_operation_catering_pkey", x => x.id_catering_mode_operation_catering);
                    table.ForeignKey(
                        name: "fk_catering_mode_operation_catering",
                        column: x => x.id_catering,
                        principalTable: "catering",
                        principalColumn: "id_catering",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_catering_mode_operation_mode",
                        column: x => x.id_mode_operation_catering,
                        principalTable: "mode_operation_catering",
                        principalColumn: "id_mode_operation_catering",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "photo_sight",
                columns: table => new
                {
                    id_photo_sight = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    link_photo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    short_description = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    id_sight = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("photo_sight_pkey", x => x.id_photo_sight);
                    table.ForeignKey(
                        name: "fk_photo_sight_sight",
                        column: x => x.id_sight,
                        principalTable: "sight",
                        principalColumn: "id_sight",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sight_operating_mode",
                columns: table => new
                {
                    id_sight_operating_mode = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_sight = table.Column<int>(type: "integer", nullable: false),
                    id_operating_mode = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("sight_operating_mode_pkey", x => x.id_sight_operating_mode);
                    table.ForeignKey(
                        name: "fk_sight_operating_mode_operating",
                        column: x => x.id_operating_mode,
                        principalTable: "operating_mode",
                        principalColumn: "id_operating_mode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_sight_operating_mode_sight",
                        column: x => x.id_sight,
                        principalTable: "sight",
                        principalColumn: "id_sight",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "catering_type_kitchen",
                columns: table => new
                {
                    id_catering_type_kitchen = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_catering = table.Column<int>(type: "integer", nullable: false),
                    id_type_kitchen = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("catering_type_kitchen_pkey", x => x.id_catering_type_kitchen);
                    table.ForeignKey(
                        name: "fk_catering_type_kitchen_catering",
                        column: x => x.id_catering,
                        principalTable: "catering",
                        principalColumn: "id_catering",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_catering_type_kitchen_type",
                        column: x => x.id_type_kitchen,
                        principalTable: "type_kitchen",
                        principalColumn: "id_type_kitchen",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "feedback",
                columns: table => new
                {
                    id_feedback = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    message_subject = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    text_message = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    contact_communication_number = table.Column<string>(type: "character(11)", fixedLength: true, maxLength: 11, nullable: false),
                    id_user = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("feedback_pkey", x => x.id_feedback);
                    table.ForeignKey(
                        name: "fk_feedback_user",
                        column: x => x.id_user,
                        principalTable: "userbot",
                        principalColumn: "id_user",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "route",
                columns: table => new
                {
                    id_route = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    date_creation = table.Column<DateOnly>(type: "date", nullable: false),
                    name_route = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    id_user = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("route_pkey", x => x.id_route);
                    table.ForeignKey(
                        name: "fk_route_userbot",
                        column: x => x.id_user,
                        principalTable: "userbot",
                        principalColumn: "id_user");
                });

            migrationBuilder.CreateTable(
                name: "route_catering_hotel",
                columns: table => new
                {
                    id_route_catering_hotel = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_route = table.Column<int>(type: "integer", nullable: false),
                    id_catering = table.Column<int>(type: "integer", nullable: true),
                    id_hotel = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("route_catering_hotel_pkey", x => x.id_route_catering_hotel);
                    table.ForeignKey(
                        name: "fk_route_catering_hotel_catering",
                        column: x => x.id_catering,
                        principalTable: "catering",
                        principalColumn: "id_catering",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_route_catering_hotel_hotel",
                        column: x => x.id_hotel,
                        principalTable: "hotel",
                        principalColumn: "id_hotel",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_route_catering_hotel_route",
                        column: x => x.id_route,
                        principalTable: "route",
                        principalColumn: "id_route",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "route_event_sight",
                columns: table => new
                {
                    id_route_event_sight = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_route = table.Column<int>(type: "integer", nullable: false),
                    id_event = table.Column<int>(type: "integer", nullable: true),
                    id_sight = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("route_event_sight_pkey", x => x.id_route_event_sight);
                    table.ForeignKey(
                        name: "fk_route_event_sight_event",
                        column: x => x.id_event,
                        principalTable: "event_",
                        principalColumn: "id_event",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_route_event_sight_route",
                        column: x => x.id_route,
                        principalTable: "route",
                        principalColumn: "id_route",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_route_event_sight_sight",
                        column: x => x.id_sight,
                        principalTable: "sight",
                        principalColumn: "id_sight",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_catering_mode_operation_catering_id_catering",
                table: "catering_mode_operation_catering",
                column: "id_catering");

            migrationBuilder.CreateIndex(
                name: "IX_catering_mode_operation_catering_id_mode_operation_catering",
                table: "catering_mode_operation_catering",
                column: "id_mode_operation_catering");

            migrationBuilder.CreateIndex(
                name: "IX_catering_type_kitchen_id_catering",
                table: "catering_type_kitchen",
                column: "id_catering");

            migrationBuilder.CreateIndex(
                name: "IX_catering_type_kitchen_id_type_kitchen",
                table: "catering_type_kitchen",
                column: "id_type_kitchen");

            migrationBuilder.CreateIndex(
                name: "IX_feedback_id_user",
                table: "feedback",
                column: "id_user");

            migrationBuilder.CreateIndex(
                name: "IX_photo_sight_id_sight",
                table: "photo_sight",
                column: "id_sight");

            migrationBuilder.CreateIndex(
                name: "IX_route_id_user",
                table: "route",
                column: "id_user");

            migrationBuilder.CreateIndex(
                name: "IX_route_catering_hotel_id_catering",
                table: "route_catering_hotel",
                column: "id_catering");

            migrationBuilder.CreateIndex(
                name: "IX_route_catering_hotel_id_hotel",
                table: "route_catering_hotel",
                column: "id_hotel");

            migrationBuilder.CreateIndex(
                name: "IX_route_catering_hotel_id_route",
                table: "route_catering_hotel",
                column: "id_route");

            migrationBuilder.CreateIndex(
                name: "IX_route_event_sight_id_event",
                table: "route_event_sight",
                column: "id_event");

            migrationBuilder.CreateIndex(
                name: "IX_route_event_sight_id_route",
                table: "route_event_sight",
                column: "id_route");

            migrationBuilder.CreateIndex(
                name: "IX_route_event_sight_id_sight",
                table: "route_event_sight",
                column: "id_sight");

            migrationBuilder.CreateIndex(
                name: "IX_sight_operating_mode_id_operating_mode",
                table: "sight_operating_mode",
                column: "id_operating_mode");

            migrationBuilder.CreateIndex(
                name: "IX_sight_operating_mode_id_sight",
                table: "sight_operating_mode",
                column: "id_sight");

            migrationBuilder.CreateIndex(
                name: "IX_ticket_id_event",
                table: "ticket",
                column: "id_event");

            migrationBuilder.CreateIndex(
                name: "type_kitchen_name_type_kitchen_key",
                table: "type_kitchen",
                column: "name_type_kitchen",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "catering_mode_operation_catering");

            migrationBuilder.DropTable(
                name: "catering_type_kitchen");

            migrationBuilder.DropTable(
                name: "feedback");

            migrationBuilder.DropTable(
                name: "photo_sight");

            migrationBuilder.DropTable(
                name: "route_catering_hotel");

            migrationBuilder.DropTable(
                name: "route_event_sight");

            migrationBuilder.DropTable(
                name: "sight_operating_mode");

            migrationBuilder.DropTable(
                name: "souvenir");

            migrationBuilder.DropTable(
                name: "special_day_catering");

            migrationBuilder.DropTable(
                name: "special_day_sight");

            migrationBuilder.DropTable(
                name: "ticket");

            migrationBuilder.DropTable(
                name: "mode_operation_catering");

            migrationBuilder.DropTable(
                name: "type_kitchen");

            migrationBuilder.DropTable(
                name: "catering");

            migrationBuilder.DropTable(
                name: "hotel");

            migrationBuilder.DropTable(
                name: "route");

            migrationBuilder.DropTable(
                name: "operating_mode");

            migrationBuilder.DropTable(
                name: "sight");

            migrationBuilder.DropTable(
                name: "event_");

            migrationBuilder.DropTable(
                name: "userbot");

            migrationBuilder.DropSequence(
                name: "catering_mode_operation_catering_id_catering_mode_operation_cat");

            migrationBuilder.DropSequence(
                name: "catering_type_kitchen_id_catering_type_kitchen_seq");

            migrationBuilder.DropSequence(
                name: "event_id_event_seq");

            migrationBuilder.DropSequence(
                name: "feedback_id_feedback_seq");

            migrationBuilder.DropSequence(
                name: "mode_operation_catering_id_mode_operation_catering_seq");

            migrationBuilder.DropSequence(
                name: "operating_mode_id_operating_mode_seq");

            migrationBuilder.DropSequence(
                name: "photo_sight_id_photo_sight_seq");

            migrationBuilder.DropSequence(
                name: "route_catering_hotel_id_route_catering_hotel_seq");

            migrationBuilder.DropSequence(
                name: "route_event_sight_id_route_event_sight_seq");

            migrationBuilder.DropSequence(
                name: "route_id_route_seq");

            migrationBuilder.DropSequence(
                name: "sight_id_sight_seq");

            migrationBuilder.DropSequence(
                name: "sight_operating_mode_id_sight_operating_mode_seq");

            migrationBuilder.DropSequence(
                name: "souvenir_id_souvenir_seq");

            migrationBuilder.DropSequence(
                name: "special_day_catering_id_special_day_catering_seq");

            migrationBuilder.DropSequence(
                name: "special_day_sight_id_special_day_sight_seq");

            migrationBuilder.DropSequence(
                name: "ticket_id_ticket_seq");

            migrationBuilder.DropSequence(
                name: "type_kitchen_id_type_kitchen_seq");
        }
    }
}
