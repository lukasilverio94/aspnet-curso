using System.ComponentModel.DataAnnotations;

public class UserDetails
{
    [Required(ErrorMessage = "Username is required")]
    public string Name { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }

}