using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TCVWeb.Migrations
{
    public partial class InitData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppSettings",
                columns: table => new
                {
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    Value = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSettings", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    IsLocked = table.Column<bool>(nullable: false),
                    OpenTime = table.Column<DateTime>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    CreateIP = table.Column<string>(maxLength: 60, nullable: true),
                    LastLogin = table.Column<DateTime>(nullable: true),
                    LastLoginIP = table.Column<string>(maxLength: 60, nullable: true),
                    LastUpdate = table.Column<DateTime>(nullable: true),
                    UpdateUser = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShopAttribs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    Title = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopAttribs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Taxonomies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ParentId = table.Column<int>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Taxonomies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Taxonomies_Taxonomies_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Taxonomies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserThreads",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MsgCount = table.Column<int>(nullable: false),
                    TotalRating = table.Column<float>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserThreads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<int>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountLog",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    Action = table.Column<int>(nullable: false),
                    RemoteIP = table.Column<string>(maxLength: 60, nullable: true),
                    LogData = table.Column<string>(nullable: true),
                    LogTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountLog_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    RoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MediaAlbums",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: true),
                    ShortName = table.Column<string>(maxLength: 64, nullable: false),
                    FullName = table.Column<string>(maxLength: 128, nullable: false),
                    Description = table.Column<string>(maxLength: 256, nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaAlbums", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaAlbums_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Shippings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Address = table.Column<string>(maxLength: 256, nullable: true),
                    City = table.Column<string>(maxLength: 64, nullable: true),
                    State = table.Column<string>(maxLength: 64, nullable: true),
                    Country = table.Column<string>(maxLength: 64, nullable: true),
                    ZipCode = table.Column<string>(maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shippings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shippings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShopCarts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopCarts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopCarts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShopWishes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopWishes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopWishes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Deliver",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: true),
                    ThreadId = table.Column<int>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Address = table.Column<string>(maxLength: 256, nullable: true),
                    City = table.Column<string>(maxLength: 64, nullable: true),
                    State = table.Column<string>(maxLength: 64, nullable: true),
                    Country = table.Column<string>(maxLength: 64, nullable: true),
                    ZipCode = table.Column<string>(maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deliver", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Deliver_UserThreads_ThreadId",
                        column: x => x.ThreadId,
                        principalTable: "UserThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Deliver_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: true),
                    ThreadId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Address = table.Column<string>(maxLength: 256, nullable: true),
                    City = table.Column<string>(maxLength: 64, nullable: true),
                    State = table.Column<string>(maxLength: 64, nullable: true),
                    Country = table.Column<string>(maxLength: 64, nullable: true),
                    ZipCode = table.Column<string>(maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Suppliers_UserThreads_ThreadId",
                        column: x => x.ThreadId,
                        principalTable: "UserThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Suppliers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserMessages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    ThreadId = table.Column<int>(nullable: false),
                    ParentId = table.Column<int>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    Rating = table.Column<int>(nullable: false),
                    Like = table.Column<int>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserMessages_UserMessages_ParentId",
                        column: x => x.ParentId,
                        principalTable: "UserMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserMessages_UserThreads_ThreadId",
                        column: x => x.ThreadId,
                        principalTable: "UserThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserMessages_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlogPosts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AlbumId = table.Column<int>(nullable: true),
                    ThreadId = table.Column<int>(nullable: true),
                    CreateUser = table.Column<string>(maxLength: 128, nullable: true),
                    UpdateUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    LastUpdate = table.Column<DateTime>(nullable: true),
                    PublishTime = table.Column<DateTime>(nullable: true),
                    Title = table.Column<string>(maxLength: 128, nullable: false),
                    TitleEn = table.Column<string>(maxLength: 128, nullable: true),
                    Image = table.Column<string>(maxLength: 512, nullable: true),
                    ExtLink = table.Column<string>(maxLength: 1024, nullable: true),
                    Preview = table.Column<string>(maxLength: 1024, nullable: true),
                    PreviewEn = table.Column<string>(maxLength: 1024, nullable: true),
                    Content = table.Column<string>(nullable: true),
                    ContentEn = table.Column<string>(nullable: true),
                    Format = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlogPosts_MediaAlbums_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "MediaAlbums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BlogPosts_UserThreads_ThreadId",
                        column: x => x.ThreadId,
                        principalTable: "UserThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MediaFiles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AlbumId = table.Column<int>(nullable: false),
                    FileName = table.Column<string>(maxLength: 128, nullable: true),
                    FullPath = table.Column<string>(maxLength: 512, nullable: true),
                    FileSize = table.Column<long>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaFiles_MediaAlbums_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "MediaAlbums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShopItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SKU = table.Column<string>(maxLength: 32, nullable: true),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    NameEn = table.Column<string>(maxLength: 64, nullable: false),
                    Image = table.Column<string>(maxLength: 512, nullable: true),
                    Preview = table.Column<string>(maxLength: 1024, nullable: true),
                    PreviewEn = table.Column<string>(maxLength: 1024, nullable: true),
                    Content = table.Column<string>(nullable: true),
                    ContentEn = table.Column<string>(nullable: true),
                    RegularPrice = table.Column<double>(nullable: false),
                    RegularPriceEn = table.Column<double>(nullable: false),
                    SalePrice = table.Column<double>(nullable: false),
                    SalePriceEn = table.Column<double>(nullable: false),
                    Packaging = table.Column<string>(nullable: true),
                    PackagingEn = table.Column<string>(nullable: true),
                    Specifications = table.Column<string>(nullable: true),
                    SpecificationsEn = table.Column<string>(nullable: true),
                    AlbumId = table.Column<int>(nullable: true),
                    ThreadId = table.Column<int>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    LastUpdate = table.Column<DateTime>(nullable: true),
                    PublishTime = table.Column<DateTime>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopItems_MediaAlbums_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "MediaAlbums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShopItems_UserThreads_ThreadId",
                        column: x => x.ThreadId,
                        principalTable: "UserThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShopOrders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: true),
                    DeliverId = table.Column<int>(nullable: true),
                    ShippingId = table.Column<int>(nullable: true),
                    AdjustPrice = table.Column<double>(nullable: false),
                    ShippingFee = table.Column<double>(nullable: false),
                    GrandTotalPrice = table.Column<double>(nullable: false),
                    PaymentInfo = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    DeliveryTime = table.Column<DateTime>(nullable: true),
                    Note = table.Column<string>(maxLength: 256, nullable: true),
                    OrderStatus = table.Column<int>(nullable: false),
                    PaymentStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopOrders_Deliver_DeliverId",
                        column: x => x.DeliverId,
                        principalTable: "Deliver",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShopOrders_Shippings_ShippingId",
                        column: x => x.ShippingId,
                        principalTable: "Shippings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShopOrders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BlogPostTaxoes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TaxoId = table.Column<int>(nullable: false),
                    PostId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPostTaxoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlogPostTaxoes_BlogPosts_PostId",
                        column: x => x.PostId,
                        principalTable: "BlogPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlogPostTaxoes_Taxonomies_TaxoId",
                        column: x => x.TaxoId,
                        principalTable: "Taxonomies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShopCartItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CartId = table.Column<int>(nullable: false),
                    ItemId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    ItemAttrib = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopCartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopCartItems_ShopCarts_CartId",
                        column: x => x.CartId,
                        principalTable: "ShopCarts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShopCartItems_ShopItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "ShopItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShopItemAttribs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AttrId = table.Column<int>(nullable: false),
                    ItemId = table.Column<int>(nullable: false),
                    IsRequired = table.Column<bool>(nullable: false),
                    Values = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopItemAttribs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopItemAttribs_ShopAttribs_AttrId",
                        column: x => x.AttrId,
                        principalTable: "ShopAttribs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShopItemAttribs_ShopItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "ShopItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShopItemTaxoes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TaxoId = table.Column<int>(nullable: false),
                    ItemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopItemTaxoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopItemTaxoes_ShopItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "ShopItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShopItemTaxoes_Taxonomies_TaxoId",
                        column: x => x.TaxoId,
                        principalTable: "Taxonomies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShopWishItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    WishId = table.Column<int>(nullable: false),
                    ItemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopWishItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopWishItems_ShopItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "ShopItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShopWishItems_ShopWishes_WishId",
                        column: x => x.WishId,
                        principalTable: "ShopWishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShopOrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OrderId = table.Column<int>(nullable: false),
                    ItemId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    ItemAttrib = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopOrderItems_ShopItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "ShopItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShopOrderItems_ShopOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "ShopOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountLog_Action",
                table: "AccountLog",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_AccountLog_LogTime",
                table: "AccountLog",
                column: "LogTime");

            migrationBuilder.CreateIndex(
                name: "IX_AccountLog_UserId",
                table: "AccountLog",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_AlbumId",
                table: "BlogPosts",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_ThreadId",
                table: "BlogPosts",
                column: "ThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostTaxoes_PostId",
                table: "BlogPostTaxoes",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostTaxoes_TaxoId",
                table: "BlogPostTaxoes",
                column: "TaxoId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliver_ThreadId",
                table: "Deliver",
                column: "ThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliver_UserId",
                table: "Deliver",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaAlbums_UserId",
                table: "MediaAlbums",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_AlbumId",
                table: "MediaFiles",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_Shippings_UserId",
                table: "Shippings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopCartItems_CartId",
                table: "ShopCartItems",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopCartItems_ItemId",
                table: "ShopCartItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopCarts_UserId",
                table: "ShopCarts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopItemAttribs_AttrId",
                table: "ShopItemAttribs",
                column: "AttrId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopItemAttribs_ItemId",
                table: "ShopItemAttribs",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopItems_AlbumId",
                table: "ShopItems",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopItems_Name",
                table: "ShopItems",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ShopItems_SKU",
                table: "ShopItems",
                column: "SKU",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShopItems_ThreadId",
                table: "ShopItems",
                column: "ThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopItemTaxoes_ItemId",
                table: "ShopItemTaxoes",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopItemTaxoes_TaxoId",
                table: "ShopItemTaxoes",
                column: "TaxoId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopOrderItems_ItemId",
                table: "ShopOrderItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopOrderItems_OrderId",
                table: "ShopOrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopOrders_DeliverId",
                table: "ShopOrders",
                column: "DeliverId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopOrders_ShippingId",
                table: "ShopOrders",
                column: "ShippingId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopOrders_UserId",
                table: "ShopOrders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopWishes_UserId",
                table: "ShopWishes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopWishItems_ItemId",
                table: "ShopWishItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopWishItems_WishId",
                table: "ShopWishItems",
                column: "WishId");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_ThreadId",
                table: "Suppliers",
                column: "ThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_UserId",
                table: "Suppliers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Taxonomies_ParentId",
                table: "Taxonomies",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Taxonomies_Name_Type",
                table: "Taxonomies",
                columns: new[] { "Name", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_UserMessages_ParentId",
                table: "UserMessages",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMessages_ThreadId",
                table: "UserMessages",
                column: "ThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMessages_UserId",
                table: "UserMessages",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountLog");

            migrationBuilder.DropTable(
                name: "AppSettings");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BlogPostTaxoes");

            migrationBuilder.DropTable(
                name: "MediaFiles");

            migrationBuilder.DropTable(
                name: "ShopCartItems");

            migrationBuilder.DropTable(
                name: "ShopItemAttribs");

            migrationBuilder.DropTable(
                name: "ShopItemTaxoes");

            migrationBuilder.DropTable(
                name: "ShopOrderItems");

            migrationBuilder.DropTable(
                name: "ShopWishItems");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropTable(
                name: "UserMessages");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "BlogPosts");

            migrationBuilder.DropTable(
                name: "ShopCarts");

            migrationBuilder.DropTable(
                name: "ShopAttribs");

            migrationBuilder.DropTable(
                name: "Taxonomies");

            migrationBuilder.DropTable(
                name: "ShopOrders");

            migrationBuilder.DropTable(
                name: "ShopItems");

            migrationBuilder.DropTable(
                name: "ShopWishes");

            migrationBuilder.DropTable(
                name: "Deliver");

            migrationBuilder.DropTable(
                name: "Shippings");

            migrationBuilder.DropTable(
                name: "MediaAlbums");

            migrationBuilder.DropTable(
                name: "UserThreads");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
