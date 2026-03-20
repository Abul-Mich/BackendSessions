namespace DbApi.DTOs
{
    public class CreateStudentDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public int BirthYear { get; set; }
        public string Country { get; set; }
    }
}
