using IdermaGerencia.Datos;
using IdermaGerencia.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace IdermaGerencia.Pages;

public sealed partial class ResumenPage : Page
{
    private bool _controlesListos;

    public ResumenPage()
    {
        InitializeComponent();
        CitasLista.ItemsSource = RepositorioEjemplo.Citas;
        ActividadLista.ItemsSource = RepositorioEjemplo.Actividad;
        CargarIndicadores();
    }

    protected override void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        ExigirImagen.Toggled -= Control_Toggled;
        AvisarVencimientos.Toggled -= Control_Toggled;
        ExigirImagen.IsOn = ControlesEjemplo.ExigirConsentimientoImagen;
        AvisarVencimientos.IsOn = ControlesEjemplo.AvisarVencimientos;
        ExigirImagen.Toggled += Control_Toggled;
        AvisarVencimientos.Toggled += Control_Toggled;
        _controlesListos = true;
        CitasLista.ItemsSource = RepositorioEjemplo.Citas;
        CargarIndicadores();
        RefrescarPendientes();
        ActualizarControles();
    }

    private void CargarIndicadores()
    {
        var todos = RepositorioEjemplo.Colaboradores;
        var trabajadores = todos.Where(c => c.Rol == RolClinica.Trabajador).ToList();
        var doctores = todos.Where(c => c.Rol == RolClinica.Doctor).ToList();

        KpiTrabajadores.Text = trabajadores.Count(c => c.EstaActivo).ToString();
        KpiTrabajadoresDetalle.Text = $"{trabajadores.Count(c => !c.EstaActivo)} en el historial de retiros";
        KpiDoctores.Text = doctores.Count(c => c.EstaActivo).ToString();
        KpiDoctoresDetalle.Text = $"{doctores.Count(c => !c.EstaActivo)} en el historial de retiros";
        KpiPlanilla.Text = Formato.Dinero(RepositorioEjemplo.Compromiso(ModalidadPago.Planilla));
        KpiHonorarios.Text = Formato.Dinero(RepositorioEjemplo.Compromiso(ModalidadPago.ReciboPorHonorarios));

        OcupacionBarra.Maximum = Math.Max(RepositorioEjemplo.CuposDia, RepositorioEjemplo.Citas.Count);
        OcupacionBarra.Value = RepositorioEjemplo.Citas.Count;
        OcupacionTexto.Text = $"{RepositorioEjemplo.Citas.Count} reuniones en la agenda.";
        PintarSedes();
    }

    private void PintarSedes()
    {
        var sedes = RepositorioEjemplo.Sedes;
        PintarSede(sedes.ElementAtOrDefault(0), SedeCard1, SedeNombre1, SedeEnfoque1, SedeHorario1, SedePersonal1, SedeEstado1);
        PintarSede(sedes.ElementAtOrDefault(1), SedeCard2, SedeNombre2, SedeEnfoque2, SedeHorario2, SedePersonal2, SedeEstado2);
    }

    private static void PintarSede(
        SedeInfo? sede,
        FrameworkElement tarjeta,
        TextBlock nombre,
        TextBlock enfoque,
        TextBlock horario,
        TextBlock personal,
        TextBlock estado)
    {
        tarjeta.Visibility = sede is null ? Visibility.Collapsed : Visibility.Visible;
        if (sede is null)
            return;

        nombre.Text = sede.Nombre;
        enfoque.Text = sede.Enfoque;
        horario.Text = sede.Horario;
        personal.Text = sede.PersonalTexto;
        estado.Text = sede.Estado;
    }

    private void Control_Toggled(object sender, RoutedEventArgs e)
    {
        if (!_controlesListos)
            return;

        ControlesEjemplo.ExigirConsentimientoImagen = ExigirImagen.IsOn;
        ControlesEjemplo.AvisarVencimientos = AvisarVencimientos.IsOn;
        RefrescarPendientes();
        ActualizarControles();
    }

    private void RefrescarPendientes()
    {
        if (PendientesLista is null || VerAlertas is null)
            return;

        var visibles = RepositorioEjemplo.AlertasVisibles();
        PendientesLista.ItemsSource = visibles.Take(4).ToList();
        VerAlertas.Content = visibles.Count == 1 ? "Ver 1 alerta" : $"Ver las {visibles.Count} alertas";
    }

    private void ActualizarControles()
    {
        if (EstadoControles is null)
            return;

        var imagen = ControlesEjemplo.ExigirConsentimientoImagen
            ? "Las campañas nuevas exigen autorización de imagen."
            : "La exigencia de autorización de imagen está apagada.";
        var avisos = ControlesEjemplo.AvisarVencimientos
            ? "Se avisan contratos y consentimientos que vencen en 45 días."
            : "Los avisos de vencimiento están ocultos.";
        EstadoControles.Text = $"{imagen} {avisos}";
    }

    private void PendientesLista_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is Alerta alerta)
            NavegacionApp.AbrirAlerta(alerta);
    }

    private void Cierre_Click(object sender, RoutedEventArgs e)
    {
        AvisoAccion.Title = "Acción de ejemplo";
        AvisoAccion.Message = "El cierre de octubre todavía no se envía a contabilidad. El botón queda listo para cuando exista esa conexión.";
        AvisoAccion.IsOpen = true;
    }

    private void IrFinanzas_Click(object sender, RoutedEventArgs e) => NavegacionApp.Ir("finanzas");
    private void IrAgenda_Click(object sender, RoutedEventArgs e) => NavegacionApp.Ir("agenda");
    private void IrAlertas_Click(object sender, RoutedEventArgs e) => NavegacionApp.Ir("alertas");
    private void IrMarketing_Click(object sender, RoutedEventArgs e) => NavegacionApp.Ir("marketing");
}
