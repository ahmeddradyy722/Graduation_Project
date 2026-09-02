using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.Data.Models
{
    public class DiabetesPrediction
    {
        public int Id { get; set; }
        public string PatientName { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public bool Hypertension { get; set; }
        public bool HeartDisease { get; set; }
        public string SmokingHistory { get; set; }
        public float BMI { get; set; }
        public float HbA1cLevel { get; set; }
        public float BloodGlucoseLevel { get; set; }
        public string PredictionResult { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("User")]
        public string? UserId { get; set; }

        public ApplicationUser? User { get; set; }


    }
}
