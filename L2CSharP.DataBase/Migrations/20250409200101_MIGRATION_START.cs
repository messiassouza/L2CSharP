using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace L2CSharP.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class MIGRATION_START : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PassWordCrypted = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PassWord = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastServer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clan",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClanId = table.Column<int>(type: "int", nullable: false),
                    ClanName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RepScore = table.Column<int>(type: "int", nullable: false),
                    LeaderObjectId = table.Column<int>(type: "int", nullable: false),
                    CrestId = table.Column<int>(type: "int", nullable: false),
                    ClanLevel = table.Column<int>(type: "int", nullable: false),
                    AllianceId = table.Column<int>(type: "int", nullable: false),
                    AllianceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AllianceCrestId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameServers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Port = table.Column<int>(type: "int", nullable: false),
                    IsTestServer = table.Column<int>(type: "int", nullable: false),
                    Brackets = table.Column<int>(type: "int", nullable: false),
                    ServerHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameServers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Npc",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CollisionRadius = table.Column<int>(type: "int", nullable: false),
                    CollisionHeight = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AttackRange = table.Column<int>(type: "int", nullable: false),
                    Hp = table.Column<int>(type: "int", nullable: false),
                    Mp = table.Column<int>(type: "int", nullable: false),
                    HpReg = table.Column<int>(type: "int", nullable: false),
                    MpReg = table.Column<int>(type: "int", nullable: false),
                    Str = table.Column<int>(type: "int", nullable: false),
                    Con = table.Column<int>(type: "int", nullable: false),
                    Dex = table.Column<int>(type: "int", nullable: false),
                    Int = table.Column<int>(type: "int", nullable: false),
                    Wit = table.Column<int>(type: "int", nullable: false),
                    Men = table.Column<int>(type: "int", nullable: false),
                    Exp = table.Column<int>(type: "int", nullable: false),
                    Sp = table.Column<int>(type: "int", nullable: false),
                    Patk = table.Column<int>(type: "int", nullable: false),
                    Pdef = table.Column<int>(type: "int", nullable: false),
                    Matk = table.Column<int>(type: "int", nullable: false),
                    Mdef = table.Column<int>(type: "int", nullable: false),
                    AtkSpd = table.Column<int>(type: "int", nullable: false),
                    Aggro = table.Column<int>(type: "int", nullable: false),
                    MatkSpd = table.Column<int>(type: "int", nullable: false),
                    Rhand = table.Column<int>(type: "int", nullable: false),
                    Lhand = table.Column<int>(type: "int", nullable: false),
                    Enchant = table.Column<int>(type: "int", nullable: false),
                    WalkSpd = table.Column<int>(type: "int", nullable: false),
                    RunSpd = table.Column<int>(type: "int", nullable: false),
                    DropHerbGroup = table.Column<int>(type: "int", nullable: false),
                    BaseStats = table.Column<int>(type: "int", nullable: false),
                    IsAttackable = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Npc", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Weapon",
                columns: table => new
                {
                    ItemId = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdditionalName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BodyPart = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Crystallizable = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Weight = table.Column<short>(type: "smallint", nullable: false),
                    Soulshots = table.Column<byte>(type: "tinyint", nullable: false),
                    Spiritshots = table.Column<byte>(type: "tinyint", nullable: false),
                    Material = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CrystalType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PDam = table.Column<short>(type: "smallint", nullable: false),
                    RndDam = table.Column<short>(type: "smallint", nullable: false),
                    WeaponType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Critical = table.Column<short>(type: "smallint", nullable: false),
                    HitModify = table.Column<short>(type: "smallint", nullable: false),
                    AvoidModify = table.Column<short>(type: "smallint", nullable: false),
                    ShieldDef = table.Column<short>(type: "smallint", nullable: false),
                    ShieldDefRate = table.Column<short>(type: "smallint", nullable: false),
                    AtkSpeed = table.Column<short>(type: "smallint", nullable: false),
                    MpConsume = table.Column<byte>(type: "tinyint", nullable: false),
                    MDam = table.Column<short>(type: "smallint", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    Time = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false),
                    CrystalCount = table.Column<short>(type: "smallint", nullable: false),
                    Sellable = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dropable = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Destroyable = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tradeable = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Depositable = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Enchant4SkillId = table.Column<short>(type: "smallint", nullable: false),
                    Enchant4SkillLvl = table.Column<byte>(type: "tinyint", nullable: false),
                    OnCastSkillId = table.Column<short>(type: "smallint", nullable: false),
                    OnCastSkillLvl = table.Column<byte>(type: "tinyint", nullable: false),
                    OnCastSkillChance = table.Column<short>(type: "smallint", nullable: false),
                    OnCritSkillId = table.Column<short>(type: "smallint", nullable: false),
                    OnCritSkillLvl = table.Column<byte>(type: "tinyint", nullable: false),
                    OnCritSkillChance = table.Column<short>(type: "smallint", nullable: false),
                    ChangeWeaponId = table.Column<short>(type: "smallint", nullable: false),
                    Skill = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weapon", x => x.ItemId);
                });

            migrationBuilder.CreateTable(
                name: "Character",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObjId = table.Column<int>(type: "int", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CharacterName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Exp = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurHP = table.Column<int>(type: "int", nullable: false),
                    CurMP = table.Column<int>(type: "int", nullable: false),
                    CurCP = table.Column<int>(type: "int", nullable: false),
                    SP = table.Column<int>(type: "int", nullable: false),
                    PkKills = table.Column<int>(type: "int", nullable: false),
                    PvpKills = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    Online = table.Column<int>(type: "int", nullable: false),
                    Karma = table.Column<int>(type: "int", nullable: false),
                    HairColor = table.Column<int>(type: "int", nullable: false),
                    HairStyle = table.Column<int>(type: "int", nullable: false),
                    Face = table.Column<int>(type: "int", nullable: false),
                    Heading = table.Column<int>(type: "int", nullable: false),
                    IsGM = table.Column<int>(type: "int", nullable: false),
                    X = table.Column<int>(type: "int", nullable: false),
                    Y = table.Column<int>(type: "int", nullable: false),
                    Z = table.Column<int>(type: "int", nullable: false),
                    IsHero = table.Column<int>(type: "int", nullable: false),
                    IsNoble = table.Column<int>(type: "int", nullable: false),
                    ClanId = table.Column<long>(type: "bigint", nullable: true),
                    Race = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Character", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Character_Clan_ClanId",
                        column: x => x.ClanId,
                        principalTable: "Clan",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SpawnList",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NpcTemplateId = table.Column<long>(type: "bigint", nullable: false),
                    LocX = table.Column<int>(type: "int", nullable: false),
                    LocY = table.Column<int>(type: "int", nullable: false),
                    LocZ = table.Column<int>(type: "int", nullable: false),
                    Heading = table.Column<int>(type: "int", nullable: false),
                    RespawnDelay = table.Column<int>(type: "int", nullable: false),
                    PeriodOfDay = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpawnList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpawnList_Npc_NpcTemplateId",
                        column: x => x.NpcTemplateId,
                        principalTable: "Npc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterItem",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ObjId = table.Column<int>(type: "int", nullable: false),
                    IsEquipped = table.Column<int>(type: "int", nullable: false),
                    EnchantLevel = table.Column<int>(type: "int", nullable: false),
                    AugmentId = table.Column<int>(type: "int", nullable: false),
                    Mana = table.Column<int>(type: "int", nullable: false),
                    TimeLimit = table.Column<int>(type: "int", nullable: false),
                    PlayerObjId = table.Column<long>(type: "bigint", nullable: false),
                    ItemCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterItem_Character_PlayerObjId",
                        column: x => x.PlayerObjId,
                        principalTable: "Character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterShortcut",
                columns: table => new
                {
                    CharId = table.Column<long>(type: "bigint", nullable: false),
                    Slot = table.Column<long>(type: "bigint", nullable: false),
                    Page = table.Column<long>(type: "bigint", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    CharType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterShortcut", x => new { x.Id, x.CharId, x.Slot, x.Page });
                    table.ForeignKey(
                        name: "FK_CharacterShortcut_Character_CharId",
                        column: x => x.CharId,
                        principalTable: "Character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterSkill",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CharId = table.Column<long>(type: "bigint", nullable: false),
                    SkillId = table.Column<int>(type: "int", nullable: false),
                    SkillLevel = table.Column<int>(type: "int", nullable: false),
                    EnchantLevel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterSkill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterSkill_Character_CharId",
                        column: x => x.CharId,
                        principalTable: "Character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Character_ClanId",
                table: "Character",
                column: "ClanId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterItem_PlayerObjId",
                table: "CharacterItem",
                column: "PlayerObjId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterShortcut_CharId",
                table: "CharacterShortcut",
                column: "CharId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterSkill_CharId",
                table: "CharacterSkill",
                column: "CharId");

            migrationBuilder.CreateIndex(
                name: "IX_SpawnList_NpcTemplateId",
                table: "SpawnList",
                column: "NpcTemplateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "CharacterItem");

            migrationBuilder.DropTable(
                name: "CharacterShortcut");

            migrationBuilder.DropTable(
                name: "CharacterSkill");

            migrationBuilder.DropTable(
                name: "GameServers");

            migrationBuilder.DropTable(
                name: "SpawnList");

            migrationBuilder.DropTable(
                name: "Weapon");

            migrationBuilder.DropTable(
                name: "Character");

            migrationBuilder.DropTable(
                name: "Npc");

            migrationBuilder.DropTable(
                name: "Clan");
        }
    }
}
