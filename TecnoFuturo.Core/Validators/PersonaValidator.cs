using System.Text.RegularExpressions;
using TecnoFuturo.Core.Entities;

namespace TecnoFuturo.Core.Validators;

public partial class PersonaValidator : IValidator<Persona>
{
    public bool Validate(Persona entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Nif) || !NifRegex().IsMatch(entity.Nif))
            return false;
        if (string.IsNullOrWhiteSpace(entity.Nombre))
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(entity.Email) &&
            EmailRegex().IsMatch(entity.Email))
        {
            return false;
        }

        return true;
    }

    [GeneratedRegex(@"^([0-9]{8}|[XYZxyz][0-9]{7})[a-zA-Z]$")]
    private static partial Regex NifRegex();
    [GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
    private static partial Regex EmailRegex();
}