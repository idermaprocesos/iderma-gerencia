using IdermaGerencia.Datos;
using IdermaGerencia.Models;
using Microsoft.UI.Xaml.Controls;

namespace IdermaGerencia.Pages;

public sealed partial class AlertasPage : Page
{
    public AlertasPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        var alertas = RepositorioEjemplo.AlertasVisibles();
        Lista.ItemsSource = alertas;
        var aviso = ControlesEjemplo.AvisarVencimientos
            ? "Incluye vencimientos de los próximos 45 días."
            : "Los vencimientos están ocultos desde el panel de control.";
        Subtitulo.Text = $"{alertas.Count} pendientes. {aviso} Abre una fila para ir al contrato o a la sección.";
    }

    private void Lista_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is Alerta alerta)
            NavegacionApp.AbrirAlerta(alerta);
    }
}
