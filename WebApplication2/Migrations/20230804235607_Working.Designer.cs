﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApplication2.Data;

#nullable disable

namespace WebApplication2.Migrations
{
    [DbContext(typeof(WebApplication2Context))]
    [Migration("20230804235607_Working")]
    partial class Working
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("WebApplication2.Models.Brand", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Brands");
                });

            modelBuilder.Entity("WebApplication2.Models.Laptop", b =>
                {
                    b.Property<Guid>("Number")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("BrandId")
                        .HasColumnType("int");

                    b.Property<int>("Condition")
                        .HasColumnType("int");

                    b.Property<int>("InStockQuantity")
                        .HasColumnType("int");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<Guid>("StoreNumber")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("StoreNumber1")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Number");

                    b.HasIndex("BrandId");

                    b.HasIndex("StoreNumber1");

                    b.ToTable("Laptops");
                });

            modelBuilder.Entity("WebApplication2.Models.Store", b =>
                {
                    b.Property<Guid>("StoreNumber")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Province")
                        .HasColumnType("int");

                    b.Property<string>("StreetNameAndNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("StoreNumber");

                    b.ToTable("Store");
                });

            modelBuilder.Entity("WebApplication2.Models.Laptop", b =>
                {
                    b.HasOne("WebApplication2.Models.Brand", "Brand")
                        .WithMany("Laptops")
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebApplication2.Models.Store", "Store")
                        .WithMany("Laptops")
                        .HasForeignKey("StoreNumber1")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Brand");

                    b.Navigation("Store");
                });

            modelBuilder.Entity("WebApplication2.Models.Brand", b =>
                {
                    b.Navigation("Laptops");
                });

            modelBuilder.Entity("WebApplication2.Models.Store", b =>
                {
                    b.Navigation("Laptops");
                });
#pragma warning restore 612, 618
        }
    }
}
