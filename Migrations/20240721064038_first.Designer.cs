﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace IliaDabirkhane.Migrations
{
    [DbContext(typeof(Context))]
    [Migration("20240721064038_first")]
    partial class first
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Atteched", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int?>("Id"));

                    b.Property<DateTime?>("CreateDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("FileName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FilePath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FileType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("MessageId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MessageId");

                    b.ToTable("Attecheds_tbl");
                });

            modelBuilder.Entity("MessageLog", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int?>("Id"));

                    b.Property<DateTime?>("CreateDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("LogAction")
                        .HasColumnType("int");

                    b.Property<int>("MessageId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MessageId");

                    b.HasIndex("UserId");

                    b.ToTable("msgLog_tbl");
                });

            modelBuilder.Entity("Messages", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int?>("Id"));

                    b.Property<string>("BodyText")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreateDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Deleted")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("SenderUserId")
                        .HasColumnType("int");

                    b.Property<string>("SerialNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Subject")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Trashed")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("SenderUserId");

                    b.ToTable("Messages_tbl");
                });

            modelBuilder.Entity("Recivers", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int?>("Id"));

                    b.Property<DateTime?>("CreateDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("MessageId")
                        .HasColumnType("int");

                    b.Property<int?>("ReciverId")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("MessageId");

                    b.HasIndex("ReciverId");

                    b.ToTable("Recivers_tbl");
                });

            modelBuilder.Entity("UserLog", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int?>("Id"));

                    b.Property<DateTime?>("CreateDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("LogAction")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<bool>("isSucces")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("userLogs_tbl");
                });

            modelBuilder.Entity("Users", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int?>("Id"));

                    b.Property<string>("Addres")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreateDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NatinalCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PerconalCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Profile")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Token")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users_tbl");
                });

            modelBuilder.Entity("smsToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("smsTokens");
                });

            modelBuilder.Entity("smsUser", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int?>("Id"));

                    b.Property<DateTime?>("CreateDateTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsValid")
                        .HasColumnType("bit");

                    b.Property<string>("SmsCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TryCount")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("sms_tbl");
                });

            modelBuilder.Entity("Atteched", b =>
                {
                    b.HasOne("Messages", "Message")
                        .WithMany("Atteched")
                        .HasForeignKey("MessageId");

                    b.Navigation("Message");
                });

            modelBuilder.Entity("MessageLog", b =>
                {
                    b.HasOne("Messages", "Message")
                        .WithMany()
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Users", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Message");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Messages", b =>
                {
                    b.HasOne("Users", "SenderUser")
                        .WithMany()
                        .HasForeignKey("SenderUserId");

                    b.Navigation("SenderUser");
                });

            modelBuilder.Entity("Recivers", b =>
                {
                    b.HasOne("Messages", "Message")
                        .WithMany("Recivers")
                        .HasForeignKey("MessageId");

                    b.HasOne("Users", "Reciver")
                        .WithMany()
                        .HasForeignKey("ReciverId");

                    b.Navigation("Message");

                    b.Navigation("Reciver");
                });

            modelBuilder.Entity("UserLog", b =>
                {
                    b.HasOne("Users", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Messages", b =>
                {
                    b.Navigation("Atteched");

                    b.Navigation("Recivers");
                });
#pragma warning restore 612, 618
        }
    }
}
