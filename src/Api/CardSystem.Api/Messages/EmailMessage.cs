using System.ComponentModel.DataAnnotations;

namespace CardSystem.Api.Messages;

public record EmailMessage([EmailAddress] string Email);