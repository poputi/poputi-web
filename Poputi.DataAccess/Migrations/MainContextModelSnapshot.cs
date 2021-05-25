﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Poputi.DataAccess.Contexts;

namespace Poputi.DataAccess.Migrations
{
    [DbContext(typeof(MainContext))]
    partial class MainContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasPostgresExtension("postgis")
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.4")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("Poputi.DataAccess.Daos.Car", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("Capacity")
                        .HasColumnType("integer");

                    b.Property<int>("DriverId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("DriverId");

                    b.ToTable("Car");
                });

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

                    b.Property<int>("CityRouteType")
                        .HasColumnType("integer");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("timestamp without time zone");

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

            modelBuilder.Entity("Poputi.DataAccess.Daos.RouteMatch", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int?>("DriverId")
                        .HasColumnType("integer");

                    b.Property<int?>("MatchedCityRouteId")
                        .HasColumnType("integer");

                    b.Property<int>("RouteMatchType")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("DriverId");

                    b.HasIndex("MatchedCityRouteId");

                    b.ToTable("RouteMatches");
                });

            modelBuilder.Entity("Poputi.DataAccess.Daos.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FirstMidName")
                        .HasColumnType("text");

                    b.Property<int?>("HomeCityId")
                        .HasColumnType("integer");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<string>("Login")
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.Property<Guid?>("RouteMatchId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("HomeCityId");

                    b.HasIndex("RouteMatchId");

                    b.ToTable("Users");

                    b.HasDiscriminator<string>("Discriminator").HasValue("User");
                });

            modelBuilder.Entity("Poputi.DataAccess.Daos.Driver", b =>
                {
                    b.HasBaseType("Poputi.DataAccess.Daos.User");

                    b.HasDiscriminator().HasValue("Driver");
                });

            modelBuilder.Entity("Poputi.DataAccess.Daos.Car", b =>
                {
                    b.HasOne("Poputi.DataAccess.Daos.Driver", "Driver")
                        .WithMany("Cars")
                        .HasForeignKey("DriverId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Driver");
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

            modelBuilder.Entity("Poputi.DataAccess.Daos.RouteMatch", b =>
                {
                    b.HasOne("Poputi.DataAccess.Daos.Driver", "Driver")
                        .WithMany()
                        .HasForeignKey("DriverId");

                    b.HasOne("Poputi.DataAccess.Daos.CityRoute", "MatchedCityRoute")
                        .WithMany()
                        .HasForeignKey("MatchedCityRouteId");

                    b.Navigation("Driver");

                    b.Navigation("MatchedCityRoute");
                });

            modelBuilder.Entity("Poputi.DataAccess.Daos.User", b =>
                {
                    b.HasOne("Poputi.DataAccess.Daos.City", "HomeCity")
                        .WithMany()
                        .HasForeignKey("HomeCityId");

                    b.HasOne("Poputi.DataAccess.Daos.RouteMatch", null)
                        .WithMany("FellowTravelers")
                        .HasForeignKey("RouteMatchId");

                    b.Navigation("HomeCity");
                });

            modelBuilder.Entity("Poputi.DataAccess.Daos.RouteMatch", b =>
                {
                    b.Navigation("FellowTravelers");
                });

            modelBuilder.Entity("Poputi.DataAccess.Daos.User", b =>
                {
                    b.Navigation("HostedRoutes");
                });

            modelBuilder.Entity("Poputi.DataAccess.Daos.Driver", b =>
                {
                    b.Navigation("Cars");
                });
#pragma warning restore 612, 618
        }
    }
}
