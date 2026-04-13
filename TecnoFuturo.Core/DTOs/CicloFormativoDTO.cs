using TecnoFuturo.Core.Entities;

namespace TecnoFuturo.Core.DTOs;

public record CicloFormativoDTO
    (
        int CentroId,
        string CicloFormativoId,
        string Nombre,
        Turno Turno
        );