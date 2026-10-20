using IdermaGerencia.Datos;
using IdermaGerencia.Dialogos;
using IdermaGerencia.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace IdermaGerencia.Pages;

public sealed partial class AgendaPage : Page
{
    public AgendaPage()
    {
        InitializeComponent();
        Refrescar();
    }

    private async void NuevaCita_Click(object sender, RoutedEventArgs e)
    {
        var cita = new CitaDia
        {
            Inicio = RepositorioEjemplo.Corte.Date.AddHours(9),
            Asunto = "",
            Sala = "Sala de reuniones",
            Estado = "Confirmada",
        };
        var dialogo = new EditorCitaDialog(cita, nueva: true) { XamlRoot = XamlRoot };
        if (await dialogo.ShowAsync() != ContentDialogResult.Primary)
            return;

        var avisoEnviado = Notificaciones.AvisarNuevaCita(cita);
        AvisoCita.Message = avisoEnviado
            ? $"{cita.Cuando} · {cita.Asunto}. {cita.ConteoTexto}. También se envió una notificación del sistema."
            : $"{cita.Cuando} · {cita.Asunto}. {cita.ConteoTexto}. La reunión quedó en la agenda; la notificación del sistema no pudo mostrarse.";
        AvisoCita.IsOpen = true;
        Refrescar();
    }

    private async void CitasLista_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is not CitaDia cita)
            return;

        var dialogo = new EditorCitaDialog(cita, nueva: false) { XamlRoot = XamlRoot };
        if (await dialogo.ShowAsync() == ContentDialogResult.Primary)
            Refrescar();
    }

    private void Refrescar()
    {
        var citas = RepositorioEjemplo.Citas;
        CitasLista.ItemsSource = citas;
        OcupacionBarra.Maximum = Math.Max(RepositorioEjemplo.CuposDia, citas.Count);
        OcupacionBarra.Value = citas.Count;
        OcupacionTexto.Text = citas.Count == 1
            ? "1 reunión en la agenda."
            : $"{citas.Count} reuniones en la agenda.";
    }
}
