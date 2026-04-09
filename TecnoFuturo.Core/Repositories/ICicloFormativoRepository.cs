using TecnoFuturo.Core.Entities;

namespace TecnoFuturo.Core.Repositories;

public interface ICicloFormativoRepository
{
    IReadOnlyList<CicloFormativo> ObtenerCiclosFormativos();
    IReadOnlyList<CicloFormativo> ObtenerCiclosFormativosPorCentro(int centroId);
    CicloFormativo? ObtenerCicloFormativoPorId(string id);
    CicloFormativo InsertarCicloFormativo(CicloFormativo cicloFormativo);
    CicloFormativo ModificarCicloFormativo(CicloFormativo cicloFormativo);
    bool BorrarCicloFormativo(string id);
}