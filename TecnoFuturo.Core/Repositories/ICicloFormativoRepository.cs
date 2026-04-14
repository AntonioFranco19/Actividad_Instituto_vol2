using TecnoFuturo.Core.DTOs;
using TecnoFuturo.Core.Entities;

namespace TecnoFuturo.Core.Repositories;

public interface ICicloFormativoRepository
{
    IReadOnlyList<CicloFormativoDTO> ObtenerCiclosFormativos();
    IReadOnlyList<CicloFormativoDTO> ObtenerCiclosFormativosPorCentro(int centroId);
    CicloFormativoDTO? ObtenerCicloFormativoPorId(string id);
    CicloFormativoDTO InsertarCicloFormativo(CicloFormativo cicloFormativo);
    CicloFormativoDTO ModificarCicloFormativo(CicloFormativo cicloFormativo);
    bool BorrarCicloFormativo(string id);
}