using FluentValidation;
using System.ComponentModel.DataAnnotations;
using Prictionary.Configuration;

namespace Prictionary.DTOs;

public class CredentialsForm
{
    /// <summary>
    /// Influenses how <see cref="Identifier"/> should be treated.
    /// </summary>
    [Required]
    public IdentificationType IdentificationType { get; set; }

    /// <summary>
    /// Universal identifier. Should be treated according to <see cref="IdentificationType"/> value.
    /// </summary>
    [Required]
    public required string Identifier { get; set; }

    [Required]
    public required string Password { get; set; }
}

public class CredentialsFormValidator : AbstractValidator<CredentialsForm>
{
    public CredentialsFormValidator() 
    {
        RuleFor(credentials => credentials.Identifier)
            .EmailAddress()
            .When(credentials => credentials.IdentificationType is IdentificationType.email);
    }
}
