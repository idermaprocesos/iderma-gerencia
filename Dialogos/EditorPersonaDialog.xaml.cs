using IdermaGerencia.Datos;
using IdermaGerencia.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace IdermaGerencia.Dialogos;

public sealed partial class EditorPersonaDialog : ContentDialog
{
    private readonly Colaborador _persona;
    private readonly bool _nueva;

    public EditorPersonaDialog(Colaborador persona, bool nueva)
    {
        InitializeComponent();
        _persona = persona;
        _nueva = nueva;

        var esDoctor = persona.Rol == RolClinica.Doctor;
        Title = nueva
            ? esDoctor ? "Nuevo doctor" : "Nuevo trabajador"
            : esDoctor ? "Editar doctor" : "Editar trabajador";
        BloqueMedico.Visibility = esDoctor ? Visibility.Visible : Visibility.Collapsed;

        CargarOpciones(TipoContrato, ["Plazo indeterminado", "Plazo fijo", "Locación de servicios"], persona.Contrato.TipoContrato);
        CargarOpciones(Sede, ["San Isidro", "Miraflores"], persona.Contrato.Sede);
        CargarOpciones(Jornada, ["Tiempo completo", "Medio tiempo", "Por turnos", "Por procedimiento"], persona.Contrato.Jornada);
        CargarOpciones(Modalidad, ["Planilla", "Recibo por honorarios"], persona.Contrato.ModalidadTexto);

        Nombres.Text = persona.Nombres;
        Apellidos.Text = persona.Apellidos;
        Documento.Text = persona.Documento;
        Telefono.Text = persona.Telefono;
        Correo.Text = persona.Correo;
        Colegiatura.Text = persona.Colegiatura;
        Especialidad.Text = persona.Especialidad;
        NumeroContrato.Text = persona.Contrato.NumeroContrato;
        Cargo.Text = persona.Contrato.Cargo;
        Area.Text = persona.Contrato.Area;
        Horario.Text = persona.Contrato.Horario;
        FechaIngreso.Date = ComoFecha(persona.Contrato.FechaIngreso);
        FechaFin.Date = ComoFecha(persona.Contrato.FechaFinContrato);
        SeRetiro.IsChecked = persona.Contrato.FechaRetiro is not null;
        FechaRetiro.Date = ComoFecha(persona.Contrato.FechaRetiro);
        MotivoRetiro.Text = persona.Contrato.MotivoRetiro;
        Sueldo.Value = (double)persona.Contrato.SueldoMensual;
        CodigoPago.Text = persona.Contrato.CodigoPago;
        Banco.Text = persona.Contrato.Banco;
        Cuenta.Text = persona.Contrato.NumeroCuenta;
        AceptaImagen.IsOn = persona.Contrato.AceptaUsoImagen;
        Canales.Text = persona.Contrato.CanalesImagen == "—" ? "" : persona.Contrato.CanalesImagen;
        Restricciones.Text = persona.Contrato.RestriccionesImagen == "—" ? "" : persona.Contrato.RestriccionesImagen;
        FechaConsentimiento.Date = ComoFecha(persona.Contrato.FechaConsentimientoImagen);
        VencimientoImagen.Date = ComoFecha(persona.Contrato.VencimientoImagen);
        Observaciones.Text = persona.Contrato.Observaciones;
        ActualizarRetiro();
    }

    private void Retiro_Changed(object sender, RoutedEventArgs e) => ActualizarRetiro();

    private void ActualizarRetiro()
    {
        if (BloqueRetiro is null)
            return;

        BloqueRetiro.Visibility = SeRetiro.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
    }

    private void Guardar_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var error = Validar();
        if (error is not null)
        {
            args.Cancel = true;
            ErrorTexto.Text = error;
            ErrorTexto.Visibility = Visibility.Visible;
            return;
        }

        _persona.Nombres = Nombres.Text.Trim();
        _persona.Apellidos = Apellidos.Text.Trim();
        _persona.Documento = Documento.Text.Trim();
        _persona.Telefono = Telefono.Text.Trim();
        _persona.Correo = Correo.Text.Trim();
        _persona.Colegiatura = Colegiatura.Text.Trim();
        _persona.Especialidad = Especialidad.Text.Trim();

        var contrato = _persona.Contrato;
        contrato.NumeroContrato = NumeroContrato.Text.Trim();
        contrato.TipoContrato = Texto(TipoContrato);
        contrato.Cargo = Cargo.Text.Trim();
        contrato.Area = Area.Text.Trim();
        contrato.Sede = Texto(Sede);
        contrato.Jornada = Texto(Jornada);
        contrato.Horario = Horario.Text.Trim();
        contrato.FechaIngreso = FechaIngreso.Date!.Value.Date;
        contrato.FechaFinContrato = FechaFin.Date?.Date;
        contrato.FechaRetiro = SeRetiro.IsChecked == true ? FechaRetiro.Date?.Date : null;
        contrato.MotivoRetiro = SeRetiro.IsChecked == true ? MotivoRetiro.Text.Trim() : "";
        contrato.SueldoMensual = (decimal)Sueldo.Value;
        contrato.Modalidad = Texto(Modalidad) == "Planilla"
            ? ModalidadPago.Planilla
            : ModalidadPago.ReciboPorHonorarios;
        contrato.CodigoPago = CodigoPago.Text.Trim();
        contrato.Banco = Banco.Text.Trim();
        contrato.NumeroCuenta = Cuenta.Text.Trim();
        contrato.AceptaUsoImagen = AceptaImagen.IsOn;
        contrato.CanalesImagen = Canales.Text.Trim();
        contrato.RestriccionesImagen = Restricciones.Text.Trim();
        contrato.FechaConsentimientoImagen = AceptaImagen.IsOn ? FechaConsentimiento.Date?.Date : null;
        contrato.VencimientoImagen = AceptaImagen.IsOn ? VencimientoImagen.Date?.Date : null;
        contrato.Observaciones = Observaciones.Text.Trim();

        RepositorioEjemplo.GuardarColaborador(_persona, _nueva);
    }

    private string? Validar()
    {
        if (string.IsNullOrWhiteSpace(Nombres.Text) || string.IsNullOrWhiteSpace(Apellidos.Text))
            return "Escribe nombres y apellidos.";
        if (string.IsNullOrWhiteSpace(Cargo.Text) || string.IsNullOrWhiteSpace(Area.Text))
            return "El cargo y el área son obligatorios.";
        if (FechaIngreso.Date is null)
            return "Indica la fecha de ingreso.";
        if (double.IsNaN(Sueldo.Value) || Sueldo.Value < 0)
            return "El sueldo debe ser un monto válido.";
        if (SeRetiro.IsChecked == true && FechaRetiro.Date is null)
            return "Si se retiró, indica la fecha de retiro.";
        if (SeRetiro.IsChecked == true && FechaRetiro.Date is DateTimeOffset retiro && retiro.Date < FechaIngreso.Date.Value.Date)
            return "La fecha de retiro no puede ser anterior al ingreso.";
        return null;
    }

    private static void CargarOpciones(ComboBox combo, IEnumerable<string> opciones, string actual)
    {
        combo.Items.Clear();
        foreach (var opcion in opciones)
            combo.Items.Add(opcion);

        if (!string.IsNullOrWhiteSpace(actual) && !combo.Items.Contains(actual))
            combo.Items.Insert(0, actual);

        combo.SelectedItem = string.IsNullOrWhiteSpace(actual) ? combo.Items.FirstOrDefault() : actual;
    }

    private static string Texto(ComboBox combo) => combo.SelectedItem as string ?? "";

    private static DateTimeOffset? ComoFecha(DateTime? fecha) =>
        fecha is null ? null : new DateTimeOffset(DateTime.SpecifyKind(fecha.Value, DateTimeKind.Local));
}
