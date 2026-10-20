using IdermaGerencia.Models;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;

namespace IdermaGerencia;

public static class Notificaciones
{
    private static bool _registradas;

    public static void Preparar()
    {
        if (_registradas)
            return;

        try
        {
            AppNotificationManager.Default.Register();
            _registradas = true;
        }
        catch
        {
            _registradas = false;
        }
    }

    public static bool AvisarNuevaCita(CitaDia cita)
    {
        try
        {
            Preparar();
            var aviso = new AppNotificationBuilder()
                .AddText("Nueva reunión en la agenda")
                .AddText($"{cita.Cuando} · {cita.Asunto}")
                .AddText($"{cita.PersonasTexto} · {cita.Sala}")
                .BuildNotification();
            AppNotificationManager.Default.Show(aviso);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
