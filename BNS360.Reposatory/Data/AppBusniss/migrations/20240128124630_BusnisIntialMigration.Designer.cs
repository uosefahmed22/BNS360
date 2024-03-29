﻿// <auto-generated />
using System;
using BNS360.Reposatory.Data.AppBusniss;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BNS360.Reposatory.data.appbusniss.migrations
{
    [DbContext(typeof(AppBusnissDbContext))]
    [Migration("20240128124630_BusnisIntialMigration")]
    partial class BusnisIntialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BNS360.Core.Entities.Busniss", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("About")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProfilePictureUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Busnisses");
                });

            modelBuilder.Entity("BNS360.Core.Entities.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PictureUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("BNS360.Core.Entities.Contact", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("BusnissId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("EmailAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstPhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SecoundPhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SiteUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ThirdPhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("BusnissId")
                        .IsUnique();

                    b.ToTable("Contact");
                });

            modelBuilder.Entity("BNS360.Core.Entities.Location", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("BusnissId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Latitude")
                        .HasColumnType("decimal");

                    b.Property<decimal>("Longitude")
                        .HasColumnType("decimal");

                    b.HasKey("Id");

                    b.HasIndex("BusnissId")
                        .IsUnique();

                    b.ToTable("Location");
                });

            modelBuilder.Entity("BNS360.Core.Entities.Review", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("BusnissId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("Rate")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.HasIndex("BusnissId");

                    b.ToTable("Review");
                });

            modelBuilder.Entity("BNS360.Core.Entities.WorkTime", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("BusnissId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Day")
                        .HasColumnType("int");

                    b.Property<TimeOnly>("End")
                        .HasColumnType("time");

                    b.Property<TimeOnly>("Start")
                        .HasColumnType("time");

                    b.HasKey("Id");

                    b.HasIndex("BusnissId");

                    b.ToTable("WorkTime");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("IdentityRole");

                    b.HasData(
                        new
                        {
                            Id = "68f2747d-b2f2-4624-8423-a19039ae1c84",
                            ConcurrencyStamp = "e6e306ee-2ef4-43d7-a61e-6900b9732a2a",
                            Name = "Default",
                            NormalizedName = "DEFAULT"
                        },
                        new
                        {
                            Id = "4a13a68f-a8bb-4481-a652-20f34156bdea",
                            ConcurrencyStamp = "60cf3337-9e50-4cf6-a4a6-ddd8aebd0a85",
                            Name = "BusinssOwner",
                            NormalizedName = "BUSINSSOWNER"
                        },
                        new
                        {
                            Id = "7ab5458c-f3c2-4744-9813-989a0085a3c5",
                            ConcurrencyStamp = "cafc8012-e2b5-447a-bddf-0a014848ffd1",
                            Name = "ServiceProvider",
                            NormalizedName = "SERVICEPROVIDER"
                        });
                });

            modelBuilder.Entity("BNS360.Core.Entities.Busniss", b =>
                {
                    b.HasOne("BNS360.Core.Entities.Category", null)
                        .WithMany("Busnisses")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BNS360.Core.Entities.Contact", b =>
                {
                    b.HasOne("BNS360.Core.Entities.Busniss", null)
                        .WithOne("ContactInfo")
                        .HasForeignKey("BNS360.Core.Entities.Contact", "BusnissId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BNS360.Core.Entities.Location", b =>
                {
                    b.HasOne("BNS360.Core.Entities.Busniss", null)
                        .WithOne("Location")
                        .HasForeignKey("BNS360.Core.Entities.Location", "BusnissId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BNS360.Core.Entities.Review", b =>
                {
                    b.HasOne("BNS360.Core.Entities.Busniss", null)
                        .WithMany("Reviews")
                        .HasForeignKey("BusnissId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BNS360.Core.Entities.WorkTime", b =>
                {
                    b.HasOne("BNS360.Core.Entities.Busniss", null)
                        .WithMany("WorkTime")
                        .HasForeignKey("BusnissId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BNS360.Core.Entities.Busniss", b =>
                {
                    b.Navigation("ContactInfo");

                    b.Navigation("Location");

                    b.Navigation("Reviews");

                    b.Navigation("WorkTime");
                });

            modelBuilder.Entity("BNS360.Core.Entities.Category", b =>
                {
                    b.Navigation("Busnisses");
                });
#pragma warning restore 612, 618
        }
    }
}
