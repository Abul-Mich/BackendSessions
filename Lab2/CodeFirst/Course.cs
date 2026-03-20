namespace DbApi.CodeFirst
{
    public class CfCourse
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public int TeacherId { get; set; }
        public CfTeacher Teacher { get; set; }

        public ICollection<CfEnrollment> Enrollments { get; set; }
    }
}
