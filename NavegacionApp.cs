using IdermaGerencia.Models;
using IdermaGerencia.Pages;
using Microsoft.UI.Xaml.Controls;

namespace IdermaGerencia;

public static class NavegacionApp
{
    public static Frame? Marco { get; set; }
    public static Action<string>? SeleccionarSeccion { get; set; }

    public static void Ir(string seccion) => SeleccionarSeccion?.Invoke(seccion);

    public static void AbrirFicha(string personaId) => Marco?.Navigate(typeof(FichaPage), personaId);

    public static void AbrirAlerta(Alerta alerta)
    {
        if (!string.IsNullOrEmpty(alerta.PersonaId))
            AbrirFicha(alerta.PersonaId);
        else if (!string.IsNullOrEmpty(alerta.Seccion))
            Ir(alerta.Seccion);
    }
}
