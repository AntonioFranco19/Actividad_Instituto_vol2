using TecnoFuturo.Core.Entities;

namespace TecnoFuturo.Core.Validators;

public class ProfesorValidator : IValidator<Profesor>
{
    public bool Validate(Profesor entity)
    {
        var personaValidator = new PersonaValidator();
        return personaValidator.Validate(entity);
    }
}