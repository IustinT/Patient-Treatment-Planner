﻿// <auto-generated />
using System;
using ICU.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ICU.Data.Migrations
{
    [DbContext(typeof(IcuContext))]
    partial class IcuContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.16")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ICU.Data.Models.Achievement", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime2");

                    b.Property<long>("PatientId")
                        .HasColumnType("bigint");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)")
                        .HasMaxLength(450);

                    b.HasKey("Id");

                    b.HasIndex("PatientId");

                    b.ToTable("Achievements");
                });

            modelBuilder.Entity("ICU.Data.Models.CPAX", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("BedMovement")
                        .HasColumnType("int");

                    b.Property<int>("BedToChair")
                        .HasColumnType("int");

                    b.Property<int>("Cough")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("DynamicSitting")
                        .HasColumnType("int");

                    b.Property<int>("Grip")
                        .HasColumnType("int");

                    b.Property<bool>("IsGoal")
                        .HasColumnType("bit");

                    b.Property<long>("PatientId")
                        .HasColumnType("bigint");

                    b.Property<int>("Respiratory")
                        .HasColumnType("int");

                    b.Property<int>("SitToStand")
                        .HasColumnType("int");

                    b.Property<int>("StandingBalance")
                        .HasColumnType("int");

                    b.Property<int>("Stepping")
                        .HasColumnType("int");

                    b.Property<int>("Transfer")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PatientId", "IsGoal");

                    b.ToTable("CPAXes");
                });

            modelBuilder.Entity("ICU.Data.Models.Goal", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool?>("IsMainGoal")
                        .IsRequired()
                        .HasColumnType("bit");

                    b.Property<long>("PatientId")
                        .HasColumnType("bigint");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)")
                        .HasMaxLength(450);

                    b.HasKey("Id");

                    b.HasIndex("PatientId", "IsMainGoal");

                    b.ToTable("Goals");
                });

            modelBuilder.Entity("ICU.Data.Models.ImageCategory", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Deleted")
                        .HasColumnType("bit");

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)")
                        .HasMaxLength(450);

                    b.HasKey("Id");

                    b.ToTable("ImageCategories");
                });

            modelBuilder.Entity("ICU.Data.Models.Patient", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:IdentityIncrement", 1)
                        .HasAnnotation("SqlServer:IdentitySeed", 1)
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("AdmissionDate")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<string>("Hospital")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)")
                        .HasMaxLength(450);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)")
                        .HasMaxLength(450);

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<string>("Ward")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)")
                        .HasMaxLength(450);

                    b.HasKey("Id");

                    b.ToTable("Patients");
                });

            modelBuilder.Entity("ICU.Data.Models.Achievement", b =>
                {
                    b.HasOne("ICU.Data.Models.Patient", "Patient")
                        .WithMany("Achievemts")
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ICU.Data.Models.CPAX", b =>
                {
                    b.HasOne("ICU.Data.Models.Patient", "Patient")
                        .WithMany("CPAXes")
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ICU.Data.Models.Goal", b =>
                {
                    b.HasOne("ICU.Data.Models.Patient", "Patient")
                        .WithMany("Goals")
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
