using TecnoFuturo.Core.Entities;

namespace TecnoFuturo.Core.Validators;

public class CicloFormativoValidator : IValidator<CicloFormativo>
{
    public bool Validate(CicloFormativo entity)
    {
        if (string.IsNullOrEmpty(entity.Nombre)) return false;
        return true;
    }
}