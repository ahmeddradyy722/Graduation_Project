using GraduationProject.Data.Models;
using GraduationProject.Data.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Azure.Core;
using Azure;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PredictionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PredictionController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpPost("predict")]
        public IActionResult Predict([FromBody] DiabetesInputModel input)
        {
            bool isSick = input.Hypertension ||
                          input.HeartDisease ||
                          input.BMI > 30 ||
                          input.HbA1cLevel > 6.5 ||
                          input.BloodGlucoseLevel > 140;

            string result = isSick ? "Positive" : "Negative";

            return Ok(new { result });
        }

        [HttpPost("record-external")]
        public async Task<IActionResult> RecordExternalPrediction([FromBody] DiabetesInputModel input)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var prediction = new DiabetesPrediction
            {
                PatientName = input.PatientName,
                Gender = input.Gender,
                Age = input.Age,
                Hypertension = input.Hypertension,
                HeartDisease = input.HeartDisease,
                SmokingHistory = input.SmokingHistory,
                BMI = input.BMI,
                HbA1cLevel = input.HbA1cLevel,
                BloodGlucoseLevel = input.BloodGlucoseLevel,
                PredictionResult = input.PredictionResult
            };

            _context.DiabetesPredictions.Add(prediction);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Prediction saved successfully!" });
        }

        /*[Authorize] */// The user must be logged in! 
        [HttpGet("user-history")]
        public IActionResult GetUserPredictions()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var userPredictions = _context.DiabetesPredictions
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            return Ok(userPredictions);
        }
        //[Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrediction(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var prediction = await _context.DiabetesPredictions.FirstOrDefaultAsync(p => p.Id == id);

            if (prediction == null)
                return NotFound(new { message = "Prediction not found." });

            if (prediction.UserId != userId)
                return Forbid(); 

            _context.DiabetesPredictions.Remove(prediction);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Prediction deleted successfully." });
        }
    }
}
