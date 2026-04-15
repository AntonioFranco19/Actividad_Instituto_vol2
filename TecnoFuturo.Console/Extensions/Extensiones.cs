using TecnoFuturo.Core.DTOs;
using TecnoFuturo.Core.Entities;
using TecnoFuturo.Core.Repositories;

namespace TecnoFuturo.Console.Extensions;

public static class Extensiones
{
    public static void MostarInformacion(this Centro centro)
    {
        System.Console.WriteLine(centro.ObtenerFicha());
    }
    
    public static void MostarInformacion(this CicloFormativo cicloFormativo)
    {
        System.Console.WriteLine($" -> Ciclo Formativo: {cicloFormativo.Nombre} [{cicloFormativo.CicloFormativoId}]");
        System.Console.WriteLine($" -> Turno: {cicloFormativo.Turno}");
    }

    public static void MostarCiclosFormativos(this Centro centro, ICicloFormativoRepository cicloFormativoRepository)
    {
        var ciclosFormativos = cicloFormativoRepository.ObtenerCiclosFormativosPorCentro(centro.CentroId);
        if (ciclosFormativos.Count == 0)
        {
            System.Console.WriteLine("NO HAY CICLOS FORMATIVOS");
            return;
        }

        System.Console.WriteLine(new string('-', 85));
        System.Console.WriteLine(" CICLOS FORMATIVOS");
        System.Console.WriteLine(new string('-', 85));
        foreach (var ciclosFormativo in ciclosFormativos)
        {
            System.Console.WriteLine(ciclosFormativo);
        }
    }

    public static void MostarModulos(this CicloFormativo cicloFormativo, IModuloRepository moduloRepository,
        IProfesorRepository profesorRepository)
    {
        var modulos = moduloRepository.ObtenerModulosPorCicloFormativo(cicloFormativo.CicloFormativoId);
        if (modulos.Count != 0)
        {
            System.Console.WriteLine(new string('-', 85));
            foreach (var modulo in modulos)
            {
                System.Console.WriteLine(modulo);
                if (modulo.ProfesorNif != null)
                {
                    var profesor = profesorRepository.ObtenerProfesorPorNif(modulo.ProfesorNif);
                    System.Console.WriteLine($"PROFESOR : {profesor!.Nombre}");
                }
                else
                {
                    System.Console.WriteLine("SIN PROFESOR ASIGNADO");
                }

                System.Console.WriteLine(new string('-', 85));
            }
        }
        else
        {
            System.Console.WriteLine("NO HAY MODULOS REGISTRADOS");
        }
    }

    public static void MostrarProfesores(this Centro centro, IProfesorRepository profesorRepository)
    {
        var profesores = profesorRepository.ObtenerProfesoresPorCentro(centro.CentroId);
        if (profesores.Count != 0)
        {
            System.Console.WriteLine(new string('-', 85));
            foreach (var profesor in profesores)
            {
                System.Console.WriteLine(profesor);
            }
        }
        else
        {
            System.Console.WriteLine("NO HAY PROFESORES REGISTRADOS");
        }
    }

    private static ProfesorDTO ConvertirProfesoraDto(Profesor? profesor)
    {
        if (profesor != null)
        {
            var profDto = new ProfesorDTO(profesor.Nif, profesor.Nombre, profesor.Email, profesor.Email, profesor.Telefono, profesor.CentroId);
            return profDto;
        }

        throw new ArgumentException("El profesor es nulo");
    }

    public static void MostrarAlumnos(this CicloFormativo cicloFormativo, IAlumnoRepository alumnoRepository)
    {
        var alumnos = alumnoRepository.ObtenerAlumnosPorCicloFormativo(cicloFormativo.CicloFormativoId);

        if (alumnos.Count != 0)
        {
            System.Console.WriteLine(new string('=', 102));
            System.Console.WriteLine($" ALUMNOS MATRICULADOS EN {cicloFormativo.Nombre,-50}");
            System.Console.WriteLine(new string('-', 102));
            foreach (var alumno in alumnos)
            {
                System.Console.WriteLine(alumno);
            }
            System.Console.WriteLine(new string('-', 102));

        }
        else
        {
            System.Console.WriteLine("NO HAY ALUMNOS MATRICULADOS");
        }

    }

    private static AlumnoDTO ConvertirAlumnoAdto(Alumno? p)
    {
        if (p != null)
        {
            var alumDto = new AlumnoDTO(p.Nif, p.Nombre, p.Email, p.Email, p.Telefono, p.CentroId, p.CicloFormativoId);
            return alumDto;
        }

        throw new ArgumentException("El alumno es nulo");
    }

    public static void MostrarResumen(this Centro centro, ICicloFormativoRepository cicloFormativoRepository, IAlumnoRepository alumnoRepository)
    {
        var ciclosFormativos = cicloFormativoRepository.ObtenerCiclosFormativosPorCentro(centro.CentroId);
        var alumnosCentro = alumnoRepository.ObtenerAlumnosPorCentro(centro.CentroId);
        System.Console.WriteLine($"RESUMEN DEL CENTRO : {centro.Nombre}");
        System.Console.WriteLine($" => Numero de ciclos formativos : {ciclosFormativos.Count:N0}");
        System.Console.WriteLine($" => Numero de alumnos matriculados : {alumnosCentro.Count:N0}");
        if (ciclosFormativos.Count == 0)
        {
            System.Console.WriteLine("NO HAY CICLOS FORMATIVOS");
            return;
        }

        if (alumnosCentro.Count == 0)
        {
            System.Console.WriteLine("NO HAY ALUMNOS MATRICULADOS");
            return;
        }

        foreach (var ciclosFormativo in ciclosFormativos)
        {
            var alumnosPorCiclo = alumnoRepository.ObtenerAlumnosPorCicloFormativo(ciclosFormativo.CicloFormativoId);
            System.Console.WriteLine($"Alumnos en {ciclosFormativo.Nombre} : {alumnosPorCiclo.Count:10:N0}");
        }
    }
}