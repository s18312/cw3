using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cw3.Models
{
    public class StudentDbContext : DbContext
    {
        public StudentDbContext()
        {
        }

        public StudentDbContext(DbContextOptions<StudentDbContext> options)
            : base(options)
        {
        }

        public DbSet<Enrollment> Enrollment { get; set; }
        public virtual DbSet<Student> Student { get; set; }
        public virtual DbSet<Studies> Studies { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           optionsBuilder.UseSqlServer("Data Source=db-mssql;Initial Catalog=s18291;Integrated Security=True");
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Enrollment>().HasKey(e => e.IdEnrollment).HasName("Enrollment_pk");
            modelBuilder.Entity<Enrollment>().Property(e => e.IdEnrollment).ValueGeneratedNever();
            modelBuilder.Entity<Enrollment>().Property(e => e.StartDate).HasColumnType("date");
            modelBuilder.Entity<Enrollment>().HasOne(d => d.StudyId).WithMany(p => p.Enrollment).HasForeignKey(d => d.IdStudy).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("Enrollment_Studies");
            
            modelBuilder.Entity<Student>().HasKey(e => e.IndexNumber).HasName("Student_pk");
            modelBuilder.Entity<Student>().Property(e => e.IndexNumber).HasMaxLength(100);
            modelBuilder.Entity<Student>().Property(e => e.Birthdate).HasColumnType("date");
            modelBuilder.Entity<Student>().Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Student>().Property(e => e.LastName).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Student>().Property(e => e.Password).IsRequired().HasMaxLength(150).IsFixedLength();
            modelBuilder.Entity<Student>().HasOne(d => d.EnrollmentId).WithMany(p => p.Student).HasForeignKey(d => d.IdEnrollment).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("Student_Enrollment");

            modelBuilder.Entity<Studies>().HasKey(e => e.IdStudy).HasName("Studies_pk");
            modelBuilder.Entity<Studies>().Property(e => e.IdStudy).ValueGeneratedNever();
            modelBuilder.Entity<Studies>().Property(e => e.Name).IsRequired().HasMaxLength(100);
           
        }




    }
}  


