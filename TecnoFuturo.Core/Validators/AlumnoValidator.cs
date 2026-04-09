using TecnoFuturo.Core.Entities;

namespace TecnoFuturo.Core.Validators;

public class AlumnoValidator : IValidator<Alumno>
{
    public bool Validate(Alumno entity)
    {
        var personaValidator = new PersonaValidator();
        return personaValidator.Validate(entity);
    }
}