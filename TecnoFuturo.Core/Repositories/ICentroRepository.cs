using TecnoFuturo.Core.Entities;

namespace TecnoFuturo.Core.Repositories;

public interface ICentroRepository
{
    IReadOnlyList<Centro> ObtenerCentros();
    Centro? ObtenerCentroPorId(int id);
    Centro InsertarCentro(Centro centro);
    Centro ModificarCentro(Centro centro);
    bool BorrarCentro(int id);
}