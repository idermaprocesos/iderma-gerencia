using IdermaGerencia.Datos;
using IdermaGerencia.Models;
using Microsoft.UI.Xaml.Controls;

namespace IdermaGerencia.Pages;

public sealed partial class MarketingPage : Page
{
    public MarketingPage()
    {
        InitializeComponent();
        CampanasLista.ItemsSource = RepositorioEjemplo.Campanas;
        PersonasLista.ItemsSource = RepositorioEjemplo.Colaboradores
            .OrderBy(c => c.EstaActivo ? 0 : 1)
            .ThenBy(c => c.Apellidos)
            .ToList();
    }

    protected override void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        var activos = RepositorioEjemplo.Activos.ToList();
        var autorizan = activos.Count(c => c.Contrato.AceptaUsoImagen);
        ResumenImagen.Text = $"{autorizan} de {activos.Count} personas activas autorizan el uso de su imagen para marketing.";

        var exige = ControlesEjemplo.ExigirConsentimientoImagen;
        AvisoExigencia.Severity = exige ? InfoBarSeverity.Success : InfoBarSeverity.Warning;
        AvisoExigencia.Title = exige ? "Control activo" : "Control apagado";
        AvisoExigencia.Message = exige
            ? "En esta sesión, una pieza nueva no debería publicarse si la persona no autorizó el uso de imagen."
            : "En esta sesión la autorización queda solo como consulta y no bloquea la revisión.";
    }

    private void PersonasLista_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is Colaborador persona)
            NavegacionApp.AbrirFicha(persona.Id);
    }
}
