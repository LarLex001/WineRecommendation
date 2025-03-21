using System.ComponentModel.DataAnnotations;

namespace WineRecommendation.Models
{
    public class WinePredictionResult
    {
        public int Id { get; set; }

        [Display(Name = "Fixed Acidity")]
        public float FixedAcidity { get; set; }

        [Display(Name = "Volatile Acidity")]
        public float VolatileAcidity { get; set; }

        [Display(Name = "Citric Acid")]
        public float CitricAcid { get; set; }

        [Display(Name = "Residual Sugar")]
        public float ResidualSugar { get; set; }

        [Display(Name = "Chlorides")]
        public float Chlorides { get; set; }

        [Display(Name = "Free Sulfur Dioxide")]
        public float FreeSulfurDioxide { get; set; }

        [Display(Name = "Total Sulfur Dioxide")]
        public float TotalSulfurDioxide { get; set; }

        [Display(Name = "Density")]
        public float Density { get; set; }

        [Display(Name = "pH")]
        public float PH { get; set; }

        [Display(Name = "Sulphates")]
        public float Sulphates { get; set; }

        [Display(Name = "Alcohol")]
        public float Alcohol { get; set; }

        [Display(Name = "Predicted Type")]
        public string PredictedType { get; set; } = "Unknown";

        [Display(Name = "Predicted Quality")]
        public float PredictedQuality { get; set; }

        [Display(Name = "Prediction Date")]
        public DateTime PredictionDate { get; set; }

        [Display(Name = "Used for Retraining")]
        public bool ContributedToRetraining { get; set; }
    }
}