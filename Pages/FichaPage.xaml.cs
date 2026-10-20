using IdermaGerencia.Datos;
using IdermaGerencia.Dialogos;
using IdermaGerencia.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace IdermaGerencia.Pages;

public sealed partial class FichaPage : Page
{
    private Colaborador? _persona;

    public FichaPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        Mostrar(RepositorioEjemplo.Buscar(e.Parameter as string));
    }

    private async void Editar_Click(object sender, RoutedEventArgs e)
    {
        if (_persona is null)
            return;

        var dialogo = new EditorPersonaDialog(_persona, nueva: false) { XamlRoot = XamlRoot };
        if (await dialogo.ShowAsync() == ContentDialogResult.Primary)
            Mostrar(RepositorioEjemplo.Buscar(_persona.Id));
    }

    private void Mostrar(Colaborador? persona)
    {
        _persona = persona;
        if (persona is null)
        {
            FichaRaiz.Visibility = Visibility.Collapsed;
            Vacio.Visibility = Visibility.Visible;
            return;
        }

        FichaRaiz.Visibility = Visibility.Visible;
        Vacio.Visibility = Visibility.Collapsed;
        FichaRaiz.DataContext = null;
        FichaRaiz.DataContext = persona;
        BloqueMedico.Visibility = persona.Rol == RolClinica.Doctor ? Visibility.Visible : Visibility.Collapsed;
        AvisoEstado.Severity = persona.EstaActivo ? InfoBarSeverity.Success : InfoBarSeverity.Warning;
        AvisoEstado.Title = persona.EstaActivo ? "Activo" : "Retirado";
        AvisoEstado.Message = persona.ResumenContrato;
        AvisoEstado.IsOpen = true;
    }

    private void Volver_Click(object sender, RoutedEventArgs e)
    {
        if (Frame.CanGoBack)
            Frame.GoBack();
    }
}
