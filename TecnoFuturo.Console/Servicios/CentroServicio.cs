using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TecnoFuturo.App.Configuraciones;
using TecnoFuturo.Console.Extensions;
using TecnoFuturo.Console.Helpers;
using TecnoFuturo.Core;
using TecnoFuturo.Core.Entities;
using TecnoFuturo.Core.Repositories;
using TecnoFuturo.Core.Validators;

namespace TecnoFuturo.Console.Servicios;

public class CentroServicio
{
    private readonly Centro _centro;
    private readonly ICentroRepository _centroRepository;
    private readonly IAlumnoRepository _alumnoRepository;
    private readonly IProfesorRepository _profesorRepository;
    private readonly ICicloFormativoRepository _cicloFormativoRepository;
    private readonly IModuloRepository _moduloRepository;
    private readonly ILogger<CentroServicio> _logger;

    public CentroServicio(IOptions<ConfiguracionCentro> configuracionCentro,
        ILogger<CentroServicio> logger, ICentroRepository centroRepository, IAlumnoRepository alumnoRepository, IProfesorRepository profesorRepository, ICicloFormativoRepository cicloFormativoRepository, IModuloRepository moduloRepository)
    {
        _logger = logger;
        _centroRepository = centroRepository;
        _alumnoRepository = alumnoRepository;
        _profesorRepository = profesorRepository;
        _cicloFormativoRepository = cicloFormativoRepository;
        _moduloRepository = moduloRepository;
        _centro = new Centro
        {
            CentroId = configuracionCentro.Value.Id,
            Nombre = configuracionCentro.Value.Nombre ?? "SIN CONFIGURAR",
            Direccion = configuracionCentro.Value.Direccion ?? "SIN CONFIGURAR",
            Telefono = configuracionCentro.Value.Telefono ?? "SIN CONFIGURAR",

        };
        _centroRepository.InsertarCentro(_centro);
    }

    public void Run()
    {
        _logger.LogInformation("Iniciando servicio de centro");
        CicloFormativo? cicloFormativoSeleccionado = null;
        Modulo? moduloSeleccionado = null;
        System.Console.WriteLine("BIENVENIDO AL SISTEMA DE MATRICULACION DE ALUMNOS");
        _centro.MostarInformacion();
        string? opcion;
        do
        {
            if (cicloFormativoSeleccionado != null)
            {
                System.Console.WriteLine($"CICLO FORMATIVO SELECCIONADO : {cicloFormativoSeleccionado.Nombre}");
            }

            if (moduloSeleccionado != null)
            {
                System.Console.WriteLine($"MODULO SELECCIONADO : {moduloSeleccionado.Nombre}");
            }

            Menu();
            System.Console.Write("OPCION>");
            opcion = System.Console.ReadLine();
            switch (opcion)
            {
                case "1":
                    CrearCicloFormativo();
                    break;
                case "2":
                    _centro.MostarCiclosFormativos(_cicloFormativoRepository);
                    break;
                case "3":
                    cicloFormativoSeleccionado = SeleccionarCicloFormativo();
                    if (cicloFormativoSeleccionado != null)
                    {
                        moduloSeleccionado = null;
                    }
                    else
                    {
                        System.Console.WriteLine("NO SE HA SELECCIONADO UN CICLO FORMATIVO");
                    }

                    break;
                case "4":
                    if (cicloFormativoSeleccionado != null)
                    {
                        cicloFormativoSeleccionado.MostarInformacion();
                    }
                    else
                    {
                        System.Console.WriteLine("NO SE HA SELECCIONADO UN CICLO FORMATIVO");
                    }

                    break;
                case "5":
                    if (cicloFormativoSeleccionado != null)
                    {
                        CrearModulo(cicloFormativoSeleccionado);
                    }
                    else
                    {
                        System.Console.WriteLine("NO SE HA SELECCIONADO UN CICLO FORMATIVO");
                    }
                    break;
                case "6":
                    if (cicloFormativoSeleccionado != null)
                    {
                        cicloFormativoSeleccionado.MostarModulos(_moduloRepository, _profesorRepository);
                    }
                    else
                    {
                        System.Console.WriteLine("NO SE HA SELECCIONADO UN CICLO FORMATIVO");
                    }                    
                    break;
                case "7":
                    if (cicloFormativoSeleccionado != null)
                    {
                        moduloSeleccionado = SeleccionarModulo(cicloFormativoSeleccionado);
                        if (moduloSeleccionado == null)
                        {
                            System.Console.WriteLine("NO SE HA SELECCIONADO UN MODULO");
                        }
                    }
                    else
                    {
                        System.Console.WriteLine("NO SE HA SELECCIONADO UN CICLO FORMATIVO");
                    }

                    break;
                case "8":
                    RegistrarProfesor();
                    break;
                case "9":
                    _centro.MostrarProfesores(_profesorRepository);
                    break;
                case "10":
                    if (moduloSeleccionado != null)
                    {
                        RegistrarProfesorAModulo(moduloSeleccionado);
                    }
                    else
                    {
                        System.Console.WriteLine("NO SE HA SELECCIONADO UN MODULO");
                    }
                    break;
                case "11":
                    if (cicloFormativoSeleccionado != null)
                    {
                        MatricularAlumno(cicloFormativoSeleccionado);
                    }
                    else
                    {
                        System.Console.WriteLine("NO SE HA SELECCIONADO UN CICLO FORMATIVO");
                    }

                    break;
                case "12":
                    if (cicloFormativoSeleccionado != null)
                    {
                        cicloFormativoSeleccionado.MostrarAlumnos(_alumnoRepository);
                    }
                    else
                    {
                        System.Console.WriteLine("NO SE HA SELECCIONADO UN CICLO FORMATIVO");
                    }

                    break;
                case "13":
                    _centro.MostrarResumen(_cicloFormativoRepository, _alumnoRepository);
                    break;
                case "14":
                    System.Console.WriteLine("BYE BYE!");
                    break;
                default:
                    System.Console.WriteLine("Opcion no valida.");
                    break;
            }
        } while (opcion != "14");

        _logger.LogInformation("Fin del servicio de centro");
    }


    private void Menu()
    {
        System.Console.WriteLine(new string('=', 44));
        System.Console.WriteLine("|                  M E N U                 |");
        System.Console.WriteLine(new string('-', 44));
        System.Console.WriteLine("|  1. Crear ciclo formativo                |");
        System.Console.WriteLine("|  2. Listar ciclos formativos             |");
        System.Console.WriteLine("|  3. Seleccionar ciclo formativo          |");
        System.Console.WriteLine("|  4. Mostrar ciclo formativo seleccionado |");
        System.Console.WriteLine("|  5. Crear módulo                         |");
        System.Console.WriteLine("|  6. Listar modulos                       |");
        System.Console.WriteLine("|  7. Seleccionar módulo                   |");
        System.Console.WriteLine("|  8. Registrar profesor                   |");
        System.Console.WriteLine("|  9. Mostar profesores                    |");
        System.Console.WriteLine("| 10. Registrar profesor a módulo          |");
        System.Console.WriteLine("| 11. Matricular alumno                    |");
        System.Console.WriteLine("| 12. Listar Alumnos                       |");
        System.Console.WriteLine("| 13. Resumen del centro                   |");
        System.Console.WriteLine("| 14. Salir                                |");
        System.Console.WriteLine(new string('=', 44));
    }

    private void CrearCicloFormativo()
    {
        System.Console.WriteLine("CREAR CICLO FORMATIVO");
        Turno? turno;
        var codigo = Leer.Cadena("Código :", true);
        var nombre = Leer.Cadena("Nombre :", true);
        do
        {
            System.Console.WriteLine("Selecciona el turno");
            System.Console.WriteLine("1. Matutino");
            System.Console.WriteLine("2. Vespertino");
            System.Console.WriteLine("3. Nocturno");
            turno = Leer.Numero("Turno :", true, 1, 3) switch
            {
                1 => Turno.Matutino,
                2 => Turno.Vespertino,
                3 => Turno.Nocturno,
                _ => null
            };
        } while (turno == null);

        var cicloFormativo = new CicloFormativo
        {
            CentroId = _centro.CentroId,
            CicloFormativoId = codigo!,
            Nombre = nombre!,
            Turno = turno.Value
        };
        
        var validator = new CicloFormativoValidator();
        if (validator.Validate(cicloFormativo))
        {
            try
            {

                _cicloFormativoRepository.InsertarCicloFormativo(cicloFormativo);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
                _logger.LogError(e, "Error en la validacion de datos del ciclo formativo");
            }
        }
        else
        {
            System.Console.WriteLine("ERROR EN LA VALIDACION DE DATOS DEL CICLO FORMATIVO");
        }
    }

    private CicloFormativo? SeleccionarCicloFormativo()
    {
        CicloFormativo? cicloFormativo;
        _centro.MostarCiclosFormativos(_cicloFormativoRepository);
        do
        {
            System.Console.WriteLine("SELECCIONAR CICLO FORMATIVO");
            var codigo = Leer.Cadena("Introduzca el código [CANCELAR VACIO] : ", false);
            if (string.IsNullOrEmpty(codigo))
            {
                System.Console.WriteLine("No ha seleccionado ningun ciclo formativo");
                return null;
            }

            cicloFormativo = _cicloFormativoRepository.ObtenerCicloFormativoPorId(codigo);
            if (cicloFormativo == null)
            {
                System.Console.WriteLine("El ciclo formativo no existe.");
            }

        } while (cicloFormativo == null);

        return cicloFormativo;
    }

    private void CrearModulo(CicloFormativo cicloFormativo)
    {
        try
        {

            var modulo = new Modulo
            {
                CicloFormativoId = cicloFormativo.CicloFormativoId,
                ModuloId = Leer.Numero("Nombre del modulo [1-9999] : ", true, 1, 9999)!.Value,
                Nombre = Leer.Cadena("Descripcion : ", true)!,
                Horas = Leer.Numero("Horas [1-12] : ", true, 1, 12)!.Value
            };
            
            var validator = new ModuloValidator();

            if (validator.Validate(modulo))
            {
                _moduloRepository.InsertarModulo(modulo);
            }
            else
            {
                System.Console.WriteLine("ERROR EN LA VALIDACION DE DATOS DEL MODULO");
            }
            
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            _logger.LogError(e, "Error en el registro del modulo");
        }
    }

    private Modulo? SeleccionarModulo(CicloFormativo cicloFormativo)
    {
        Modulo? modulo;
        cicloFormativo.MostarModulos(_moduloRepository, _profesorRepository);
        do
        {
            System.Console.WriteLine("SELECCIONAR MODULO");
            var codigo = Leer.Numero("Introduzca el código [CANCELAR VACIO] : ", false);
            if (codigo == null)
            {
                System.Console.WriteLine("No ha seleccionado ningún modulo");
                return null;
            }

            modulo = _moduloRepository.ObtenerModuloPorId(codigo.Value);

        } while (modulo == null);

        return modulo;
    }

    private void RegistrarProfesor()
    {
        try
        {
            var profesor = new Profesor
            {
                Nif = Leer.Cadena("Introduzca el NIF:", true, @"^([0-9]{8}|[XYZxyz][0-9]{7})[a-zA-Z]$")!,
                Nombre = Leer.Cadena("Introduzca el nombre:", true)!,
                Direccíon = Leer.Cadena("Introduzca la dirección:", true)!,
                Email = Leer.Cadena("Introduzca el e-Mail:", true,@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")!,
                Telefono = Leer.Cadena("Introduzca el teléfono:", true)!
            };
            
            var validator = new ProfesorValidator();
            if (validator.Validate(profesor))
            {
                _profesorRepository.InsertarProfesor(profesor);
            }
            else
            {
                System.Console.WriteLine("ERROR EN LA VALIDACION DE DATOS DEL PROFESOR");
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine(ex.Message);
            _logger.LogError(ex, "Error al registrar profesor");
        }
    }

    private void RegistrarProfesorAModulo(Modulo modulo)
    {
        Profesor? profesor;
        System.Console.WriteLine("REGISTRO DE PROFESOR A MODULO");
        _centro.MostrarProfesores(_profesorRepository);

        do
        {
            System.Console.WriteLine("SELECCIONAR PROFESOR");
            var nif = Leer.Cadena("Introduzca el NIF [CANCELAR VACIO] : ", false);
            if (string.IsNullOrWhiteSpace(nif))
            {
                System.Console.WriteLine("No ha seleccionado ningún profesor");
                return;
            }

            profesor = _profesorRepository.ObtenerProfesorPorNif(nif);

        } while (profesor == null);

        try
        {
            modulo.ProfesorNif = profesor.Nif;
            
            var validator = new ModuloValidator();
            if (validator.Validate(modulo))
            {
                _moduloRepository.ModificarModulo(modulo);
            }
            else
            {
                System.Console.WriteLine("ERROR EN LA VALIDACION DE DATOS DEL MODULO");
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine(ex.Message);
            _logger.LogError(ex, "Error al asignar profesor al modulo");
        }
    }

    private void MatricularAlumno(CicloFormativo cicloFormativo)
    {
        System.Console.WriteLine("NUEVO REGISTRO DE MATRICULA");
        try
        {
            var alumno = new Alumno
            {
                Nif = Leer.Cadena("NIF .......: ", true)!,
                Nombre = Leer.Cadena("Nombre ....: ", true)!,
                Direccíon = Leer.Cadena("Dirección .: ", true)!,
                Email = Leer.Cadena("e-Mail ....: ", true)!,
                Telefono = Leer.Cadena("Telefono ..: ", true)!,
                CentroId = cicloFormativo.CentroId,
                CicloFormativoId = cicloFormativo.CicloFormativoId,
            };
            
            var validator = new AlumnoValidator();
            if (validator.Validate(alumno))
            {
                _alumnoRepository.InsertarAlumno(alumno);
                System.Console.WriteLine($"ALUMNO REGISTRADO AL CICLO FORMATIVO: {cicloFormativo.Nombre}");
            }
            else
            {
                System.Console.WriteLine("ERROR EN LA VALIDACION DE DATOS DEL ALUMNO");
            }
            
        }
        catch (Exception ex)
        {
            System.Console.WriteLine(ex.Message);
            _logger.LogError(ex, "Error al registar un alumno en el ciclo formativo");
        }
    }
}