namespace SCMS.Common.DbData
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class SCMSModel : DbContext
    {
        public SCMSModel(string connectionString)
            : base(connectionString)
        {
        }

        public virtual DbSet<Course> Course { get; set; }
        public virtual DbSet<Person> Person { get; set; }
        public virtual DbSet<StudentCourse> StudentCourse { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>()
                .HasMany(e => e.StudentCourse)
                .WithRequired(e => e.Course)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.Course)
                .WithOptional(e => e.Person)
                .HasForeignKey(e => e.TeacherId);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.StudentCourse)
                .WithRequired(e => e.Person)
                .HasForeignKey(e => e.StudentId)
                .WillCascadeOnDelete(false);
        }
    }
}
