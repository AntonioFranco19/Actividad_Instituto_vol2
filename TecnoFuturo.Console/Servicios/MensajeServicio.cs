using TecnoFuturo.Core;
using TecnoFuturo.Core.Servicios;

namespace TecnoFuturo.App.Servicios;

public class MensajeServicio : IMensageServicio
{
    public void MostrarMensaje(string mensaje)
    {
        System.Console.WriteLine(mensaje);
    }
}