using IdermaGerencia.Datos;
using Microsoft.UI.Xaml.Controls;

namespace IdermaGerencia.Pages;

public sealed partial class InventarioPage : Page
{
    public InventarioPage()
    {
        InitializeComponent();
        Lista.ItemsSource = RepositorioEjemplo.Insumos;
    }
}
