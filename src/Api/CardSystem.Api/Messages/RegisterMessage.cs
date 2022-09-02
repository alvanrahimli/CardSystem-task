using System.ComponentModel.DataAnnotations;

namespace CardSystem.Api.Messages;

public record struct RegisterMessage(
    [StringLength(255, MinimumLength = 2)] string FirstName, 
    [StringLength(255, MinimumLength = 2)] string LastName,
    [EmailAddress] string Username, 
    // Minimum eight characters, at least one letter and one number
    [RegularExpression("^(?=.*[A-Za-z])(?=.*\\d)[A-Za-z\\d]{8,}$")] string Password);