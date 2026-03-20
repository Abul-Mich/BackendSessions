namespace DbApi.CodeFirst
{
    public class CfStudent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int BirthYear { get; set; }
        public string Country { get; set; }

        public ICollection<CfEnrollment> Enrollments { get; set; } = new List<CfEnrollment>();
    }
}
