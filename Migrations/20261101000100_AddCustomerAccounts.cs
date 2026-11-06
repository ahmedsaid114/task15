using CinemaProject.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskCinema.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20261101000100_AddCustomerAccounts")]
public sealed class AddCustomerAccounts : Migration
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
            entity.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("int");
            SqlServerPropertyBuilderExtensions.UseIdentityColumn(entity.Property<int>("Id"));
            entity.Property<DateTime>("CreatedUtc").HasColumnType("datetime2");
            entity.Property<string>("Email").IsRequired().HasMaxLength(256).HasColumnType("nvarchar(256)");
            entity.Property<string>("Name").IsRequired().HasMaxLength(120).HasColumnType("nvarchar(120)");
            entity.Property<string>("NormalizedEmail").IsRequired().HasMaxLength(256).HasColumnType("nvarchar(256)");
            entity.Property<string>("PasswordHash").IsRequired().HasMaxLength(512).HasColumnType("nvarchar(512)");
            entity.HasKey("Id");
            entity.HasIndex("NormalizedEmail").IsUnique();
            entity.ToTable("CustomerAccounts");
        });
#pragma warning restore 612, 618
    }

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "CustomerAccounts",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                PasswordHash = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_CustomerAccounts", x => x.Id));

        migrationBuilder.CreateIndex(
            name: "IX_CustomerAccounts_NormalizedEmail",
            table: "CustomerAccounts",
            column: "NormalizedEmail",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "CustomerAccounts");
    }
}
