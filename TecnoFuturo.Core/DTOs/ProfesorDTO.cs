namespace TecnoFuturo.Core.DTOs;

public record ProfesorDTO
    (
        string Nif, 
        string Nombre, 
        string? Email, 
        string? Direccion, 
        string? Telefono, 
        int CentroId
        );