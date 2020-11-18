﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using project_manage_system_backend.Shares;

namespace project_manage_system_backend.Migrations
{
    [DbContext(typeof(PMSContext))]
    [Migration("20201118042955_UpdateDB")]
    partial class UpdateDB
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("project_manage_system_backend.Models.ProjectModel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("OwnerAccount")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("OwnerAccount");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("project_manage_system_backend.Models.RepositoryModel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Owner")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ProjectModelID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Url")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("ProjectModelID");

                    b.ToTable("Repositories");
                });

            modelBuilder.Entity("project_manage_system_backend.Models.UserModel", b =>
                {
                    b.Property<string>("Account")
                        .HasColumnType("TEXT");

                    b.Property<string>("Authority")
                        .HasColumnType("TEXT");

                    b.Property<string>("AvatarUrl")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Account");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("project_manage_system_backend.Models.ProjectModel", b =>
                {
                    b.HasOne("project_manage_system_backend.Models.UserModel", "Owner")
                        .WithMany("Projects")
                        .HasForeignKey("OwnerAccount");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("project_manage_system_backend.Models.RepositoryModel", b =>
                {
                    b.HasOne("project_manage_system_backend.Models.ProjectModel", null)
                        .WithMany("Repositories")
                        .HasForeignKey("ProjectModelID");
                });

            modelBuilder.Entity("project_manage_system_backend.Models.ProjectModel", b =>
                {
                    b.Navigation("Repositories");
                });

            modelBuilder.Entity("project_manage_system_backend.Models.UserModel", b =>
                {
                    b.Navigation("Projects");
                });
#pragma warning restore 612, 618
        }
    }
}
