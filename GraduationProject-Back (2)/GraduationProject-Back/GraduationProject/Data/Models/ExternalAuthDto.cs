namespace GraduationProject.Data.Models
{
    public class ExternalAuthDto
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}