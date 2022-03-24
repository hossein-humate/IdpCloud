﻿// <auto-generated />
using System;
using DataProvider.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DataProvider.Migrations
{
    [DbContext(typeof(EfCoreContext))]
    [Migration("20210830171408_Add_ExpireAndRefresh_UserSession")]
    partial class Add_ExpireAndRefresh_UserSession
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.5")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Entity.BaseInfo.City", b =>
                {
                    b.Property<int>("CityId")
                        .HasColumnType("int");

                    b.Property<short>("CountryId")
                        .HasColumnType("smallint");

                    b.Property<long>("CreateDate")
                        .HasColumnType("bigint");

                    b.Property<long?>("DeleteDate")
                        .HasColumnType("bigint");

                    b.Property<string>("Latitude")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Longitude")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("UpdateDate")
                        .HasColumnType("bigint");

                    b.HasKey("CityId");

                    b.HasIndex("CountryId");

                    b.ToTable("Cities", "BaseInfo");
                });

            modelBuilder.Entity("Entity.BaseInfo.Country", b =>
                {
                    b.Property<short>("CountryId")
                        .HasColumnType("smallint");

                    b.Property<string>("CallingCode")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("CommonName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("CommonNativeName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<long>("CreateDate")
                        .HasColumnType("bigint");

                    b.Property<long?>("DeleteDate")
                        .HasColumnType("bigint");

                    b.Property<bool?>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<string>("OfficialName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("OfficialNativeName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ThreeCharacterCode")
                        .IsRequired()
                        .HasMaxLength(3)
                        .HasColumnType("nchar(3)")
                        .IsFixedLength(true);

                    b.Property<string>("TwoCharacterCode")
                        .IsRequired()
                        .HasMaxLength(2)
                        .HasColumnType("nchar(2)")
                        .IsFixedLength(true);

                    b.Property<long>("UpdateDate")
                        .HasColumnType("bigint");

                    b.HasKey("CountryId");

                    b.ToTable("Countries", "BaseInfo");
                });

            modelBuilder.Entity("Entity.BaseInfo.Currency", b =>
                {
                    b.Property<short>("CurrencyId")
                        .HasColumnType("smallint");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(5)
                        .HasColumnType("nvarchar(5)");

                    b.Property<long>("CreateDate")
                        .HasColumnType("bigint");

                    b.Property<byte>("DecimalDigits")
                        .HasColumnType("tinyint");

                    b.Property<long?>("DeleteDate")
                        .HasColumnType("bigint");

                    b.Property<bool?>("IsCryptoCurrency")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("NativeSymbol")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("PluralName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("Rounding")
                        .HasColumnType("int");

                    b.Property<string>("Symbol")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<long>("UpdateDate")
                        .HasColumnType("bigint");

                    b.HasKey("CurrencyId");

                    b.ToTable("Currencies", "BaseInfo");
                });

            modelBuilder.Entity("Entity.BaseInfo.Language", b =>
                {
                    b.Property<short>("LanguageId")
                        .HasColumnType("smallint");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("CreateDate")
                        .HasColumnType("bigint");

                    b.Property<long?>("DeleteDate")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("UpdateDate")
                        .HasColumnType("bigint");

                    b.HasKey("LanguageId");

                    b.ToTable("Languages", "BaseInfo");
                });

            modelBuilder.Entity("Entity.BaseInfo.MasterDetail", b =>
                {
                    b.Property<Guid>("MasterDetailId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("CreateDate")
                        .HasColumnType("bigint");

                    b.Property<long?>("DeleteDate")
                        .HasColumnType("bigint");

                    b.Property<Guid>("MasterId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Order")
                        .HasColumnType("int");

                    b.Property<string>("Parameter")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("SoftwareId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("UpdateDate")
                        .HasColumnType("bigint");

                    b.HasKey("MasterDetailId");

                    b.HasIndex("MasterId");

                    b.HasIndex("SoftwareId");

                    b.ToTable("MasterDetails", "BaseInfo");
                });

            modelBuilder.Entity("Entity.Identity.Address", b =>
                {
                    b.Property<Guid>("AddressId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("CityId")
                        .HasColumnType("int");

                    b.Property<bool>("Confirmed")
                        .HasColumnType("bit");

                    b.Property<short?>("CountryId")
                        .HasColumnType("smallint");

                    b.Property<long>("CreateDate")
                        .HasColumnType("bigint");

                    b.Property<long?>("DeleteDate")
                        .HasColumnType("bigint");

                    b.Property<string>("Full")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HomeNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Latitude")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Longitude")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PostalCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("State")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Street")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<long>("UpdateDate")
                        .HasColumnType("bigint");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("AddressId");

                    b.HasIndex("CityId");

                    b.HasIndex("CountryId");

                    b.HasIndex("UserId");

                    b.ToTable("Addresses", "Identity");
                });

            modelBuilder.Entity("Entity.Identity.Permission", b =>
                {
                    b.Property<Guid>("PermissionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Action")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("CreateDate")
                        .HasColumnType("bigint");

                    b.Property<long?>("DeleteDate")
                        .HasColumnType("bigint");

                    b.Property<string>("Icon")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ParentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Public")
                        .HasColumnType("bit");

                    b.Property<string>("Scope")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("SoftwareId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<short>("SortOrder")
                        .HasColumnType("smallint");

                    b.Property<byte>("Type")
                        .HasColumnType("tinyint");

                    b.Property<long>("UpdateDate")
                        .HasColumnType("bigint");

                    b.HasKey("PermissionId");

                    b.HasIndex("ParentId");

                    b.HasIndex("SoftwareId");

                    b.ToTable("Permissions", "Identity");
                });

            modelBuilder.Entity("Entity.Identity.Person", b =>
                {
                    b.Property<Guid>("PersonId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<short?>("CountryLivingId")
                        .HasColumnType("smallint");

                    b.Property<long>("CreateDate")
                        .HasColumnType("bigint");

                    b.Property<long?>("DeleteDate")
                        .HasColumnType("bigint");

                    b.Property<string>("Firstname")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte>("Gender")
                        .HasColumnType("tinyint");

                    b.Property<string>("Lastname")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Middlename")
                        .HasColumnType("nvarchar(max)");

                    b.Property<short?>("NationalityId")
                        .HasColumnType("smallint");

                    b.Property<string>("PicturePath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("SoftwareId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("UpdateDate")
                        .HasColumnType("bigint");

                    b.HasKey("PersonId");

                    b.HasIndex("CountryLivingId");

                    b.HasIndex("NationalityId");

                    b.HasIndex("SoftwareId");

                    b.ToTable("Persons", "Identity");
                });

            modelBuilder.Entity("Entity.Identity.Role", b =>
                {
                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("CreateDate")
                        .HasColumnType("bigint");

                    b.Property<long?>("DeleteDate")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsDefault")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("SoftwareId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("UpdateDate")
                        .HasColumnType("bigint");

                    b.HasKey("RoleId");

                    b.HasIndex("SoftwareId");

                    b.ToTable("Roles", "Identity");
                });

            modelBuilder.Entity("Entity.Identity.RolePermission", b =>
                {
                    b.Property<Guid>("RolePermissionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("CreateDate")
                        .HasColumnType("bigint");

                    b.Property<long?>("DeleteDate")
                        .HasColumnType("bigint");

                    b.Property<Guid>("PermissionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("UpdateDate")
                        .HasColumnType("bigint");

                    b.HasKey("RolePermissionId");

                    b.HasIndex("PermissionId");

                    b.HasIndex("RoleId");

                    b.ToTable("RolePermissions", "Identity");
                });

            modelBuilder.Entity("Entity.Identity.SocialNetwork", b =>
                {
                    b.Property<Guid>("SocialNetworkId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("CreateDate")
                        .HasColumnType("bigint");

                    b.Property<long?>("DeleteDate")
                        .HasColumnType("bigint");

                    b.Property<byte>("Provider")
                        .HasColumnType("tinyint");

                    b.Property<long>("UpdateDate")
                        .HasColumnType("bigint");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SocialNetworkId");

                    b.ToTable("SocialNetworks", "Identity");
                });

            modelBuilder.Entity("Entity.Identity.Software", b =>
                {
                    b.Property<Guid>("SoftwareId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ApiKey")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Brand")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BusinessDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("CreateDate")
                        .HasColumnType("bigint");

                    b.Property<long?>("DeleteDate")
                        .HasColumnType("bigint");

                    b.Property<long>("KeyExpire")
                        .HasColumnType("bigint");

                    b.Property<string>("LogoImage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("OwnerUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<long>("UpdateDate")
                        .HasColumnType("bigint");

                    b.HasKey("SoftwareId");

                    b.ToTable("Softwares", "Identity");
                });

            modelBuilder.Entity("Entity.Identity.User", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("CreateDate")
                        .HasColumnType("bigint");

                    b.Property<long?>("DeleteDate")
                        .HasColumnType("bigint");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("EmailConfirmationExpiry")
                        .HasColumnType("bigint");

                    b.Property<string>("EmailConfirmationSecret")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<short?>("LanguageId")
                        .HasColumnType("smallint");

                    b.Property<long?>("LastLoginDate")
                        .HasColumnType("bigint");

                    b.Property<string>("LastLoginIp")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("LoginTimes")
                        .HasColumnType("int");

                    b.Property<string>("Mobile")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("MobileConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordSalt")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("PersonId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Picture")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("RegisterDate")
                        .HasColumnType("bigint");

                    b.Property<string>("RegisterIp")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<bool>("TwoFactorEnable")
                        .HasColumnType("bit");

                    b.Property<long>("UpdateDate")
                        .HasColumnType("bigint");

                    b.Property<string>("Username")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("UserId");

                    b.HasIndex("LanguageId");

                    b.HasIndex("PersonId");

                    b.ToTable("Users", "Identity");
                });

            modelBuilder.Entity("Entity.Identity.UserPermission", b =>
                {
                    b.Property<Guid>("UserPermissionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("CreateDate")
                        .HasColumnType("bigint");

                    b.Property<long?>("DeleteDate")
                        .HasColumnType("bigint");

                    b.Property<Guid>("PermissionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("UpdateDate")
                        .HasColumnType("bigint");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserPermissionId");

                    b.HasIndex("PermissionId");

                    b.HasIndex("UserId");

                    b.ToTable("UserPermission", "Identity");
                });

            modelBuilder.Entity("Entity.Identity.UserRole", b =>
                {
                    b.Property<Guid>("UserRoleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("CreateDate")
                        .HasColumnType("bigint");

                    b.Property<long?>("DeleteDate")
                        .HasColumnType("bigint");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("UpdateDate")
                        .HasColumnType("bigint");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserRoleId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("UserRoles", "Identity");
                });

            modelBuilder.Entity("Entity.Identity.UserSoftware", b =>
                {
                    b.Property<Guid>("UserSoftwareId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("CreateDate")
                        .HasColumnType("bigint");

                    b.Property<long?>("DeleteDate")
                        .HasColumnType("bigint");

                    b.Property<Guid>("SoftwareId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("UpdateDate")
                        .HasColumnType("bigint");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserSoftwareId");

                    b.HasIndex("SoftwareId");

                    b.HasIndex("UserId");

                    b.ToTable("UserSoftwares", "Identity");
                });

            modelBuilder.Entity("Entity.Identity.Visitor", b =>
                {
                    b.Property<Guid>("VisitorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BodyContent")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Browser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("CreateDate")
                        .HasColumnType("bigint");

                    b.Property<long?>("DeleteDate")
                        .HasColumnType("bigint");

                    b.Property<string>("Device")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("ExecuteTime")
                        .HasColumnType("float");

                    b.Property<string>("Ip")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsOurUser")
                        .HasColumnType("bit");

                    b.Property<string>("Platform")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StatusCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("UpdateDate")
                        .HasColumnType("bigint");

                    b.Property<string>("UrlPath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserAgent")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("VisitorId");

                    b.HasIndex("UserId");

                    b.ToTable("Visitors", "Identity");
                });

            modelBuilder.Entity("Entity.Log.ServerActivity", b =>
                {
                    b.Property<Guid>("ServerActivityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("CreateDate")
                        .HasColumnType("bigint");

                    b.Property<long?>("DeleteDate")
                        .HasColumnType("bigint");

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StackTrace")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<long>("UpdateDate")
                        .HasColumnType("bigint");

                    b.HasKey("ServerActivityId");

                    b.ToTable("ServerActivities", "Log");
                });

            modelBuilder.Entity("Entity.SSO.UserSession", b =>
                {
                    b.Property<Guid>("UserSessionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<byte>("AuthType")
                        .HasColumnType("tinyint");

                    b.Property<long>("CreateDate")
                        .HasColumnType("bigint");

                    b.Property<long?>("DeleteDate")
                        .HasColumnType("bigint");

                    b.Property<long>("ExpireDate")
                        .HasColumnType("bigint");

                    b.Property<string>("Ip")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("SoftwareId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<byte>("Status")
                        .HasColumnType("tinyint");

                    b.Property<long>("UpdateDate")
                        .HasColumnType("bigint");

                    b.Property<string>("UserAgent")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserSessionId");

                    b.HasIndex("SoftwareId");

                    b.HasIndex("UserId");

                    b.ToTable("UserSessions", "SSO");
                });

            modelBuilder.Entity("Entity.BaseInfo.City", b =>
                {
                    b.HasOne("Entity.BaseInfo.Country", "Country")
                        .WithMany("Cities")
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Country");
                });

            modelBuilder.Entity("Entity.BaseInfo.MasterDetail", b =>
                {
                    b.HasOne("Entity.BaseInfo.MasterDetail", "Master")
                        .WithMany("Details")
                        .HasForeignKey("MasterId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Entity.Identity.Software", "Software")
                        .WithMany("MasterDetails")
                        .HasForeignKey("SoftwareId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Master");

                    b.Navigation("Software");
                });

            modelBuilder.Entity("Entity.Identity.Address", b =>
                {
                    b.HasOne("Entity.BaseInfo.City", "City")
                        .WithMany("Addresses")
                        .HasForeignKey("CityId");

                    b.HasOne("Entity.BaseInfo.Country", "Country")
                        .WithMany("Addresses")
                        .HasForeignKey("CountryId");

                    b.HasOne("Entity.Identity.User", "User")
                        .WithMany("Addresses")
                        .HasForeignKey("UserId");

                    b.Navigation("City");

                    b.Navigation("Country");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Entity.Identity.Permission", b =>
                {
                    b.HasOne("Entity.Identity.Permission", "Parent")
                        .WithMany("Childrens")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Entity.Identity.Software", "Software")
                        .WithMany("Permissions")
                        .HasForeignKey("SoftwareId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Parent");

                    b.Navigation("Software");
                });

            modelBuilder.Entity("Entity.Identity.Person", b =>
                {
                    b.HasOne("Entity.BaseInfo.Country", "CountryLiving")
                        .WithMany("CountryLivingPersons")
                        .HasForeignKey("CountryLivingId");

                    b.HasOne("Entity.BaseInfo.Country", "Nationality")
                        .WithMany("NationalityPersons")
                        .HasForeignKey("NationalityId");

                    b.HasOne("Entity.Identity.Software", "Software")
                        .WithMany()
                        .HasForeignKey("SoftwareId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CountryLiving");

                    b.Navigation("Nationality");

                    b.Navigation("Software");
                });

            modelBuilder.Entity("Entity.Identity.Role", b =>
                {
                    b.HasOne("Entity.Identity.Software", "Software")
                        .WithMany("Roles")
                        .HasForeignKey("SoftwareId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Software");
                });

            modelBuilder.Entity("Entity.Identity.RolePermission", b =>
                {
                    b.HasOne("Entity.Identity.Permission", "Permission")
                        .WithMany("RolePermissions")
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Entity.Identity.Role", "Role")
                        .WithMany("RolePermissions")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Permission");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("Entity.Identity.User", b =>
                {
                    b.HasOne("Entity.BaseInfo.Language", "Language")
                        .WithMany("Users")
                        .HasForeignKey("LanguageId");

                    b.HasOne("Entity.Identity.Person", "Person")
                        .WithMany("Users")
                        .HasForeignKey("PersonId");

                    b.Navigation("Language");

                    b.Navigation("Person");
                });

            modelBuilder.Entity("Entity.Identity.UserPermission", b =>
                {
                    b.HasOne("Entity.Identity.Permission", "Permission")
                        .WithMany("UserPermissions")
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Entity.Identity.User", "User")
                        .WithMany("UserPermissions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Permission");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Entity.Identity.UserRole", b =>
                {
                    b.HasOne("Entity.Identity.Role", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Entity.Identity.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Entity.Identity.UserSoftware", b =>
                {
                    b.HasOne("Entity.Identity.Software", "Software")
                        .WithMany("UserSoftwares")
                        .HasForeignKey("SoftwareId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Entity.Identity.User", "User")
                        .WithMany("UserSoftwares")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Software");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Entity.Identity.Visitor", b =>
                {
                    b.HasOne("Entity.Identity.User", null)
                        .WithMany("Visits")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Entity.SSO.UserSession", b =>
                {
                    b.HasOne("Entity.Identity.Software", "Software")
                        .WithMany("UserSessions")
                        .HasForeignKey("SoftwareId");

                    b.HasOne("Entity.Identity.User", "User")
                        .WithMany("UserSessions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Software");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Entity.BaseInfo.City", b =>
                {
                    b.Navigation("Addresses");
                });

            modelBuilder.Entity("Entity.BaseInfo.Country", b =>
                {
                    b.Navigation("Addresses");

                    b.Navigation("Cities");

                    b.Navigation("CountryLivingPersons");

                    b.Navigation("NationalityPersons");
                });

            modelBuilder.Entity("Entity.BaseInfo.Language", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("Entity.BaseInfo.MasterDetail", b =>
                {
                    b.Navigation("Details");
                });

            modelBuilder.Entity("Entity.Identity.Permission", b =>
                {
                    b.Navigation("Childrens");

                    b.Navigation("RolePermissions");

                    b.Navigation("UserPermissions");
                });

            modelBuilder.Entity("Entity.Identity.Person", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("Entity.Identity.Role", b =>
                {
                    b.Navigation("RolePermissions");

                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("Entity.Identity.Software", b =>
                {
                    b.Navigation("MasterDetails");

                    b.Navigation("Permissions");

                    b.Navigation("Roles");

                    b.Navigation("UserSessions");

                    b.Navigation("UserSoftwares");
                });

            modelBuilder.Entity("Entity.Identity.User", b =>
                {
                    b.Navigation("Addresses");

                    b.Navigation("UserPermissions");

                    b.Navigation("UserRoles");

                    b.Navigation("UserSessions");

                    b.Navigation("UserSoftwares");

                    b.Navigation("Visits");
                });
#pragma warning restore 612, 618
        }
    }
}
