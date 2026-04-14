namespace TecnoFuturo.Core.DTOs;

public record ModuloDTO
    (
        string CicloFormativoId,
        int ModuloId,
        string? Nombre,
        int Horas,
        string? ProfesorNif
        );