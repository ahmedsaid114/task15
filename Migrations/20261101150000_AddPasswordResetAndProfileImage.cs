using CinemaProject.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskCinema.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20261101150000_AddPasswordResetAndProfileImage")]
public sealed class AddPasswordResetAndProfileImage : Migration
{
    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "10.0.14")
            .HasAnnotation("Relational:MaxIdentifierLength", 128);
        SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

        modelBuilder.Entity("CinemaProject.Models.CustomerAccount", entity =>
        {
            entity.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
            SqlServerPropertyBuilderExtensions.UseIdentityColumn(entity.Property<int>("Id"));
            entity.Property<DateTime>("CreatedUtc").HasColumnType("datetime2");
            entity.Property<string>("Email").IsRequired().HasMaxLength(256).HasColumnType("nvarchar(256)");
            entity.Property<string>("Name").IsRequired().HasMaxLength(120).HasColumnType("nvarchar(120)");
            entity.Property<string>("NormalizedEmail").IsRequired().HasMaxLength(256).HasColumnType("nvarchar(256)");
            entity.Property<string>("PasswordHash").IsRequired().HasMaxLength(512).HasColumnType("nvarchar(512)");
            entity.Property<string>("ProfileImage").HasMaxLength(400).HasColumnType("nvarchar(400)");
            entity.HasKey("Id");
            entity.HasIndex("NormalizedEmail").IsUnique();
            entity.ToTable("CustomerAccounts");
        });

        modelBuilder.Entity("CinemaProject.Models.PasswordResetToken", entity =>
        {
            entity.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
            SqlServerPropertyBuilderExtensions.UseIdentityColumn(entity.Property<int>("Id"));
            entity.Property<int>("CustomerAccountId").HasColumnType("int");
            entity.Property<DateTime>("CreatedUtc").HasColumnType("datetime2");
            entity.Property<DateTime>("ExpiresUtc").HasColumnType("datetime2");
            entity.Property<string>("TokenHash").IsRequired().HasMaxLength(64).HasColumnType("nvarchar(64)");
            entity.Property<DateTime?>("UsedUtc").HasColumnType("datetime2");
            entity.HasKey("Id");
            entity.HasIndex("CustomerAccountId");
            entity.HasIndex("TokenHash").IsUnique();
            entity.ToTable("PasswordResetTokens");
        });

        modelBuilder.Entity("CinemaProject.Models.PasswordResetToken", entity =>
        {
            entity.HasOne("CinemaProject.Models.CustomerAccount", "CustomerAccount")
                .WithMany()
                .HasForeignKey("CustomerAccountId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            entity.Navigation("CustomerAccount");
        });
#pragma warning restore 612, 618
    }

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "ProfileImage",
            table: "CustomerAccounts",
            type: "nvarchar(400)",
            maxLength: 400,
            nullable: true);

        migrationBuilder.CreateTable(
            name: "PasswordResetTokens",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CustomerAccountId = table.Column<int>(type: "int", nullable: false),
                TokenHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                ExpiresUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                UsedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PasswordResetTokens", x => x.Id);
                table.ForeignKey(
                    name: "FK_PasswordResetTokens_CustomerAccounts_CustomerAccountId",
                    column: x => x.CustomerAccountId,
                    principalTable: "CustomerAccounts",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_PasswordResetTokens_CustomerAccountId",
            table: "PasswordResetTokens",
            column: "CustomerAccountId");

        migrationBuilder.CreateIndex(
            name: "IX_PasswordResetTokens_TokenHash",
            table: "PasswordResetTokens",
            column: "TokenHash",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "PasswordResetTokens");
        migrationBuilder.DropColumn(name: "ProfileImage", table: "CustomerAccounts");
    }
}
