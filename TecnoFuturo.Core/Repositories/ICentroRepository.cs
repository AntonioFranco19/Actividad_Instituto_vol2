using TecnoFuturo.Core.DTOs;
using TecnoFuturo.Core.Entities;

namespace TecnoFuturo.Core.Repositories;

public interface ICentroRepository
{
    IReadOnlyList<CentroDTO> ObtenerCentros();
    CentroDTO? ObtenerCentroPorId(int id);
    CentroDTO InsertarCentro(Centro centro);
    CentroDTO ModificarCentro(Centro centro);
    bool BorrarCentro(int id);
}