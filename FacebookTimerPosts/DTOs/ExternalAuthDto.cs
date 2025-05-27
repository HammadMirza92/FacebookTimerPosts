using System.ComponentModel.DataAnnotations;

namespace FacebookTimerPosts.DTOs
{
    public class ExternalAuthDto
    {
        [Required]
        public string Credential { get; set; } // Changed from IdToken to Credential

        [Required]
        public string Provider { get; set; } // "Google", "Facebook", etc.

        [Required]
        public string Action { get; set; } // "login" or "register"
    }
    public class GoogleTokenPayload
    {
        public string Sub { get; set; } // Google User ID
        public string Email { get; set; }
        public bool EmailVerified { get; set; }
        public string Name { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Picture { get; set; }
        public string Hd { get; set; } // Hosted domain
        public long Iat { get; set; } // Issued at
        public long Exp { get; set; } // Expires at
    }

    public class ExternalAuthResultDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public UserDto User { get; set; }
        public int ExpiresIn { get; set; } = 86400; // 24 hours
        public bool IsNewUser { get; set; }
    }
}
