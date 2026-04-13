namespace TecnoFuturo.Core.DTOs;

public record AlumnoDTO
(
    string Nif, 
    string Nombre, 
    string Email, 
    string Direccion, 
    string Telefono, 
    int CentroId, 
    string CicloFormativoId
    );