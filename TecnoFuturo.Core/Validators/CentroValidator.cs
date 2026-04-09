using TecnoFuturo.Core.Entities;

namespace TecnoFuturo.Core.Validators;

public class CentroValidator :IValidator<Centro>
{
    public bool Validate(Centro entity)
    {
        if (string.IsNullOrEmpty(entity.Nombre)) return false;
        return !string.IsNullOrEmpty(entity.Direccion);
    }
}