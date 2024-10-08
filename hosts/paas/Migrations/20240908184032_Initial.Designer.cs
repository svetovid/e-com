﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using btm.paas.Data;

#nullable disable

namespace btm.paas.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240908184032_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.8");

            modelBuilder.Entity("btm.paas.Models.MethodActionModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("MethodActions");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Deposit"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Withdrawal"
                        });
                });

            modelBuilder.Entity("btm.paas.Models.PaymentHistoryModel", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("PaymentReference")
                        .HasColumnType("TEXT");

                    b.Property<string>("Status")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("PaymentReference");

                    b.ToTable("PaymentHistories");
                });

            modelBuilder.Entity("btm.paas.Models.PaymentModel", b =>
                {
                    b.Property<string>("Reference")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Amount")
                        .HasColumnType("TEXT");

                    b.Property<string>("CurrencyCode")
                        .HasColumnType("TEXT");

                    b.Property<string>("CustomerReference")
                        .HasColumnType("TEXT");

                    b.Property<long>("Id")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MethodActionId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ProviderAccountName")
                        .HasColumnType("TEXT");

                    b.Property<string>("PublicPaymentId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Status")
                        .HasColumnType("TEXT");

                    b.HasKey("Reference");

                    b.HasIndex("MethodActionId");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("btm.paas.Models.PaymentHistoryModel", b =>
                {
                    b.HasOne("btm.paas.Models.PaymentModel", "Payment")
                        .WithMany("PaymentHistories")
                        .HasForeignKey("PaymentReference");

                    b.Navigation("Payment");
                });

            modelBuilder.Entity("btm.paas.Models.PaymentModel", b =>
                {
                    b.HasOne("btm.paas.Models.MethodActionModel", "MethodAction")
                        .WithMany()
                        .HasForeignKey("MethodActionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MethodAction");
                });

            modelBuilder.Entity("btm.paas.Models.PaymentModel", b =>
                {
                    b.Navigation("PaymentHistories");
                });
#pragma warning restore 612, 618
        }
    }
}
