using TecnoFuturo.Core.Entities;

namespace TecnoFuturo.Core.Validators;

public class ModuloValidator : IValidator<Modulo>
{
    public bool Validate(Modulo entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Nombre))
        {
            return false;
        }

        return true;
    }
}