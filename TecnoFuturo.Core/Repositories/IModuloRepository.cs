using TecnoFuturo.Core.DTOs;
using TecnoFuturo.Core.Entities;

namespace TecnoFuturo.Core.Repositories;

public interface IModuloRepository
{
    IReadOnlyList<ModuloDTO> ObtenerModulos();
    IReadOnlyList<ModuloDTO> ObtenerModulosPorCicloFormativo(string cicloFormativoId);
    IReadOnlyList<ModuloDTO> ObtenerModulosPorProfesor(string profesorNif);
    ModuloDTO? ObtenerModuloPorId(int id);
    ModuloDTO InsertarModulo(Modulo modulo);
    ModuloDTO ModificarModulo(Modulo modulo);
    bool BorrarModulo(int id);
}