using System.Text.RegularExpressions;

namespace TecnoFuturo.Console.Helpers;

public static class Leer
{
    public static string? Cadena(string message, bool obligatorio)
    {
        do
        {
            System.Console.Write(message);
            var variable = System.Console.ReadLine();
            if (!string.IsNullOrEmpty(variable))
            {
                return variable;
            }
            if (obligatorio)
            {
                System.Console.WriteLine("Tiene que introducer un dato");
            }
            else
            {
                break;
            }
        } while (true);

        return null;
    }

    public static string? Cadena(string message, bool obligatorio, string regex)
    {
        do
        {
            System.Console.Write(message);
            var variable = System.Console.ReadLine();
            if (!string.IsNullOrEmpty(variable))
            {
                if (Regex.IsMatch(variable, regex))
                {
                    return variable;
                }

                System.Console.WriteLine("El dato introducido no cumple con el formato requerido");
            }
            if (obligatorio)
            {
                System.Console.WriteLine("Tiene que introducer un dato");
            }
            else
            {
                break;
            }
        } while (true);

        return null;
    }


    public static int? Numero(string message, bool obligatorio)
    {
        do
        {
            System.Console.Write(message);
            var entrada = System.Console.ReadLine();
            if (string.IsNullOrEmpty(entrada))
            {
                if (!obligatorio) return null;
                System.Console.WriteLine("El dato introducido es obligatorio");
                continue;
            }

            if (int.TryParse(entrada, out var numero))
            {
                return numero;
            }

            System.Console.WriteLine("Tiene que introducir un número entero");
        } while (true);
    }

    public static int? Numero(string message, bool obligatorio, int minimo, int maximo)
    {
        do
        {
            System.Console.Write(message);
            var entrada = System.Console.ReadLine();
            if (string.IsNullOrEmpty(entrada))
            {
                if (!obligatorio) return null;
                System.Console.WriteLine("El dato introducido es obligatorio");
                continue;

            }

            if (int.TryParse(entrada, out var numero))
            {
                if (numero >= minimo && numero <= maximo) return numero;
                System.Console.WriteLine("El dato debe estar entre {0} y {1}", minimo, maximo);
                continue;

            }

            System.Console.WriteLine("Tiene que introducir un número entero");
        } while (true);
    }
}