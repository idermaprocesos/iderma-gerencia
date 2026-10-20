using IdermaGerencia.Pages;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace IdermaGerencia;

public sealed partial class MainWindow : Window
{
    private string? _seccionActual;
    private bool _yaMaximizo;

    public MainWindow()
    {
        InitializeComponent();

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
        AppWindow.SetIcon("Assets/AppIcon.ico");
        AppWindow.Title = "Iderma Capilar — Gerencia";

        NavegacionApp.Marco = NavFrame;
        NavegacionApp.SeleccionarSeccion = SeleccionarSeccion;

        Maximizar();
        Activated += Ventana_Activated;
        NavegarSeccion("resumen");
    }

    private void Ventana_Activated(object sender, WindowActivatedEventArgs args)
    {
        ReservarBotonesVentana();

        if (_yaMaximizo)
            return;

        _yaMaximizo = true;
        Maximizar();
    }

    private void ReservarBotonesVentana()
    {
        if (AppTitleBar.XamlRoot is null)
            return;

        double escala = AppTitleBar.XamlRoot.RasterizationScale;
        if (escala < 0.1)
            escala = 1;

        double derecha = AppWindow.TitleBar.RightInset / escala;
        double izquierda = AppWindow.TitleBar.LeftInset / escala;
        if (derecha > 0)
            ColumnaDerecha.Width = new GridLength(derecha);
        if (izquierda > 0)
            ColumnaIzquierda.Width = new GridLength(izquierda);
    }

    private void Maximizar()
    {
        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.PreferredMinimumWidth = 1100;
            presenter.PreferredMinimumHeight = 700;
            presenter.Maximize();
        }
    }

    private void TitleBar_PaneToggleRequested(TitleBar sender, object args)
    {
        NavView.IsPaneOpen = !NavView.IsPaneOpen;
    }

    private void TitleBar_BackRequested(TitleBar sender, object args)
    {
        if (NavFrame.CanGoBack)
            NavFrame.GoBack();
    }

    private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        if (args.InvokedItemContainer is NavigationViewItem item && item.Tag is string etiqueta)
            NavegarSeccion(etiqueta);
    }

    private void SeleccionarSeccion(string etiqueta)
    {
        foreach (var elemento in NavView.MenuItems)
        {
            if (elemento is NavigationViewItem item && item.Tag as string == etiqueta)
            {
                NavView.SelectedItem = item;
                break;
            }
        }

        NavegarSeccion(etiqueta);
    }

    private void NavegarSeccion(string etiqueta)
    {
        if (_seccionActual == etiqueta && !NavFrame.CanGoBack)
            return;

        var pagina = etiqueta switch
        {
            "agenda" => typeof(AgendaPage),
            "finanzas" => typeof(FinanzasPage),
            "inventario" => typeof(InventarioPage),
            "marketing" => typeof(MarketingPage),
            "alertas" => typeof(AlertasPage),
            _ => typeof(ResumenPage),
        };

        NavFrame.Navigate(pagina);
        NavFrame.BackStack.Clear();
        _seccionActual = etiqueta;
    }
}
