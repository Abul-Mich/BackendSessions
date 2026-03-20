using Microsoft.EntityFrameworkCore;

namespace DbApi.CodeFirst
{
    public class UniversityContext : DbContext
    {
        public UniversityContext(DbContextOptions<UniversityContext> options)
            : base(options) { }

        public DbSet<CfStudent> Students { get; set; }
        public DbSet<CfTeacher> Teachers { get; set; }
        public DbSet<CfCourse> Courses { get; set; }
        public DbSet<CfEnrollment> Enrollments { get; set; }
    }
}
