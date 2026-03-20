namespace DbApi.ViewModels
{
    public class StudentEnrollmentViewModel
    {
        public int Id { get; set; }
        public string CourseName { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }

    public class StudentViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public int BirthYear { get; set; }
        public List<StudentEnrollmentViewModel> Enrollments { get; set; }
    }
}
