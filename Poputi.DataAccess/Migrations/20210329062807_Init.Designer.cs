﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Poputi.DataAccess.Contexts;

namespace Poputi.DataAccess.Migrations
{
    [DbContext(typeof(MainContext))]
    [Migration("20210329062807_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasPostgresExtension("postgis")
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.4")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("Poputi.DataAccess.Daos.City", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<Point>("Location")
                        .HasColumnType("geography");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Cities");
                });

            modelBuilder.Entity("Poputi.DataAccess.Daos.CityRoute", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<Point>("End")
                        .HasColumnType("geography");

                    b.Property<Point>("Start")
                        .HasColumnType("geography");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("CityRoutes");
                });

            modelBuilder.Entity("Poputi.DataAccess.Daos.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("FirstMidName")
                        .HasColumnType("text");

                    b.Property<int?>("HomeCityId")
                        .HasColumnType("integer");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<string>("Login")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("HomeCityId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Poputi.DataAccess.Daos.CityRoute", b =>
                {
                    b.HasOne("Poputi.DataAccess.Daos.User", "User")
                        .WithMany("HostedRoutes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Poputi.DataAccess.Daos.User", b =>
                {
                    b.HasOne("Poputi.DataAccess.Daos.City", "HomeCity")
                        .WithMany()
                        .HasForeignKey("HomeCityId");

                    b.Navigation("HomeCity");
                });

            modelBuilder.Entity("Poputi.DataAccess.Daos.User", b =>
                {
                    b.Navigation("HostedRoutes");
                });
#pragma warning restore 612, 618
        }
    }
}