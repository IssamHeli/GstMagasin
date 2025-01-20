using System;
namespace GstMagazin.Models
{
	public class ReCaptchaResponse
	{
        public bool Success { get; set; }
        public float Score { get; set; }
        public string Action { get; set; }
        public string ChallengeTs { get; set; }
        public string Hostname { get; set; }
        public List<string>? ErrorCodes { get; set; }
    }
}

