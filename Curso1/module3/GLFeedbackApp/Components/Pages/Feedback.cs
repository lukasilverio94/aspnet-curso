using System.ComponentModel.DataAnnotations;

namespace FeedbackApp;

public class Feedback
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Comment { get; set; } = string.Empty;
}
