﻿using WineRecommendation.Models;

namespace WineRecommendation.Services
{
    public interface IWinePredictionService
    {
        Task<WinePredictionResult> PredictWineAsync(WineInputModel inputModel);
        Task<List<WinePredictionResult>> GetAllPredictionsAsync();
        Task<int> GetTotalPredictionsAsync();
        Task<int> GetUntrainedPredictionsCountAsync();
        Task<WinePredictionResult?> GetPredictionByIdAsync(int id);
    }
}