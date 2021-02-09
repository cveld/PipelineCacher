﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PipelineCacher.Entities;

namespace PipelineCacher.Entities.Migrations
{
    [DbContext(typeof(PipelineCacherDbContext))]
    partial class PipelineCacherDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.1");

            modelBuilder.Entity("PipelineCacher.Entities.AuditLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Action")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ObjectEntityId")
                        .HasColumnType("int");

                    b.Property<string>("ObjectEntityName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PatId")
                        .HasColumnType("int");

                    b.Property<int?>("SubjectEntityId")
                        .HasColumnType("int");

                    b.Property<string>("SubjectEntityName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("AuditLog");
                });

            modelBuilder.Entity("PipelineCacher.Entities.Organization", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Organizations");
                });

            modelBuilder.Entity("PipelineCacher.Entities.Pat", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Token")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Pats");
                });

            modelBuilder.Entity("PipelineCacher.Entities.Pipeline", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("AzdoId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OrganizationName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProjectName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RepositoryId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Revision")
                        .HasColumnType("int");

                    b.Property<string>("YamlPath")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Pipelines");
                });

            modelBuilder.Entity("PipelineCacher.Entities.PipelineContext", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("CommitId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Environment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastChecked")
                        .HasColumnType("datetime2");

                    b.Property<string>("Parameters")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PipelineId")
                        .HasColumnType("int");

                    b.Property<int?>("PipelineStateId")
                        .HasColumnType("int");

                    b.Property<string>("SourcecodeTree")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Stages")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TargetBranch")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("PipelineId");

                    b.HasIndex("PipelineStateId");

                    b.ToTable("PipelineContext");
                });

            modelBuilder.Entity("PipelineCacher.Entities.PipelineState", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("AzdoBuildId")
                        .HasColumnType("int");

                    b.Property<string>("Branch")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Commit")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Parameters")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PipelineId")
                        .HasColumnType("int");

                    b.Property<int>("Revision")
                        .HasColumnType("int");

                    b.Property<string>("SourcecodeTree")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Stages")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("YamlPath")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("PipelineId");

                    b.ToTable("PipelineState");
                });

            modelBuilder.Entity("PipelineCacher.Entities.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("PipelineCacher.Entities.Sourcecode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("CommitId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProviderType")
                        .HasColumnType("int");

                    b.Property<string>("RepositoryId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Sourcecode");
                });

            modelBuilder.Entity("PipelineCacher.Entities.PipelineContext", b =>
                {
                    b.HasOne("PipelineCacher.Entities.Pipeline", "Pipeline")
                        .WithMany()
                        .HasForeignKey("PipelineId");

                    b.HasOne("PipelineCacher.Entities.PipelineState", "PipelineState")
                        .WithMany()
                        .HasForeignKey("PipelineStateId");

                    b.Navigation("Pipeline");

                    b.Navigation("PipelineState");
                });

            modelBuilder.Entity("PipelineCacher.Entities.PipelineState", b =>
                {
                    b.HasOne("PipelineCacher.Entities.Pipeline", "Pipeline")
                        .WithMany()
                        .HasForeignKey("PipelineId");

                    b.Navigation("Pipeline");
                });
#pragma warning restore 612, 618
        }
    }
}
