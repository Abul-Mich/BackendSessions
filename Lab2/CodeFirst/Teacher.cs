namespace DbApi.CodeFirst
{
    public class CfTeacher
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public ICollection<CfCourse> Courses { get; set; }
    }
}
