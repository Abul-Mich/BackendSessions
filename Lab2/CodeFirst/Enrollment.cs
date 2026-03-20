namespace DbApi.CodeFirst
{
    public class CfEnrollment
    {
        public int Id { get; set; }
        public DateTime EnrollmentDate { get; set; }

        public int StudentId { get; set; }
        public CfStudent Student { get; set; }

        public int CourseId { get; set; }
        public CfCourse Course { get; set; }
    }
}
