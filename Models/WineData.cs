using System.ComponentModel.DataAnnotations;

namespace WineRecommendation.Models
{
    public class WineData
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Wine Type")]
        public string Type { get; set; } = "Red";

        [Required]
        [Display(Name = "Fixed Acidity (g/L)")]
        [Range(0.01, 30.0)]
        public float FixedAcidity { get; set; }

        [Required]
        [Display(Name = "Volatile Acidity (g/L)")]
        [Range(0.01, 5.0)]
        public float VolatileAcidity { get; set; }

        [Required]
        [Display(Name = "Citric Acid (g/L)")]
        [Range(0.0, 1.0)]
        public float CitricAcid { get; set; }

        [Required]
        [Display(Name = "Residual Sugar (g/L)")]
        [Range(0.01, 100.0)]
        public float ResidualSugar { get; set; }

        [Required]
        [Display(Name = "Chlorides (g/L)")]
        [Range(0.001, 1.0)]
        public float Chlorides { get; set; }

        [Required]
        [Display(Name = "Free Sulfur Dioxide (mg/L)")]
        [Range(1.0, 300.0)]
        public float FreeSulfurDioxide { get; set; }

        [Required]
        [Display(Name = "Total Sulfur Dioxide (mg/L)")]
        [Range(1.0, 500.0)]
        public float TotalSulfurDioxide { get; set; }

        [Required]
        [Display(Name = "Density (g/cm³)")]
        [Range(0.9, 1.1)]
        public float Density { get; set; }

        [Required]
        [Display(Name = "pH")]
        [Range(2.5, 4.5)]
        public float PH { get; set; }

        [Required]
        [Display(Name = "Sulphates (g/L)")]
        [Range(0.1, 2.0)]
        public float Sulphates { get; set; }

        [Required]
        [Display(Name = "Alcohol (%)")]
        [Range(8.0, 15.0)]
        public float Alcohol { get; set; }

        [Required]
        [Display(Name = "Quality")]
        [Range(0.0, 10.0)]
        public float Quality { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Training Data")]
        public bool IsTrainingData { get; set; }
    }
}