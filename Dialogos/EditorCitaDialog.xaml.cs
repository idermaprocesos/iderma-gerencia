using System.Collections.ObjectModel;
using IdermaGerencia.Datos;
using IdermaGerencia.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace IdermaGerencia.Dialogos;

public sealed partial class EditorCitaDialog : ContentDialog
{
    private readonly CitaDia _cita;
    private readonly bool _nueva;
    private readonly ObservableCollection<AsistenteReunion> _personas = [];
    private readonly List<string> _personal;

    public EditorCitaDialog(CitaDia cita, bool nueva)
    {
        InitializeComponent();
        _cita = cita;
        _nueva = nueva;
        Title = nueva ? "Nueva reunión" : "Editar reunión";

        _personal = RepositorioEjemplo.Colaboradores
            .Select(c => c.NombreCompleto)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(nombre => nombre, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
        Persona.ItemsSource = _personal;

        Sala.ItemsSource = new List<string>
        {
            "Sala de reuniones",
            "Administración",
            "Sede San Isidro",
            "Sede Miraflores",
            "Quirófano",
        };
        Estado.ItemsSource = new List<string> { "Confirmada", "En curso", "Pendiente", "Realizada", "Cancelada" };

        Fecha.Date = new DateTimeOffset(DateTime.SpecifyKind(cita.Inicio.Date, DateTimeKind.Local));
        Hora.Time = cita.Inicio.TimeOfDay;
        Asunto.Text = cita.Asunto;
        Sala.Text = string.IsNullOrWhiteSpace(cita.Sala) ? "Sala de reuniones" : cita.Sala;
        Estado.SelectedItem = string.IsNullOrWhiteSpace(cita.Estado) ? "Confirmada" : cita.Estado;

        foreach (var nombre in cita.Personas.Where(nombre => !string.IsNullOrWhiteSpace(nombre)))
            _personas.Add(new AsistenteReunion { Nombre = nombre.Trim() });

        PersonasLista.ItemsSource = _personas;
        _personas.CollectionChanged += (_, _) => ActualizarListaVacia();
        ActualizarListaVacia();
    }

    private void AgregarPersona_Click(object sender, RoutedEventArgs e)
    {
        var nombre = Persona.Text.Trim();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            MostrarError("Escribe o elige una persona.");
            return;
        }

        var canonico = _personal.FirstOrDefault(item => item.Equals(nombre, StringComparison.OrdinalIgnoreCase)) ?? nombre;
        if (_personas.Any(item => item.Nombre.Equals(canonico, StringComparison.OrdinalIgnoreCase)))
        {
            MostrarError("Esa persona ya está en la reunión.");
            return;
        }

        _personas.Add(new AsistenteReunion { Nombre = canonico });
        Persona.Text = string.Empty;
        OcultarError();
    }

    private void QuitarPersona_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: AsistenteReunion asistente })
            return;

        _personas.Remove(asistente);
        OcultarError();
    }

    private void Guardar_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (Fecha.Date is null)
        {
            MostrarError("Indica la fecha de la reunión.");
            args.Cancel = true;
            return;
        }

        if (string.IsNullOrWhiteSpace(Asunto.Text))
        {
            MostrarError("Indica el asunto de la reunión.");
            args.Cancel = true;
            return;
        }

        if (_personas.Count == 0)
        {
            MostrarError("Agrega al menos una persona.");
            args.Cancel = true;
            return;
        }

        _cita.Inicio = Fecha.Date.Value.Date.Add(Hora.Time);
        _cita.Asunto = Asunto.Text.Trim();
        _cita.Sala = string.IsNullOrWhiteSpace(Sala.Text) ? "Sin lugar" : Sala.Text.Trim();
        _cita.Estado = Estado.SelectedItem as string ?? "Confirmada";
        _cita.Personas = _personas.Select(item => item.Nombre).ToList();
        RepositorioEjemplo.GuardarCita(_cita, _nueva);
    }

    private void ActualizarListaVacia() =>
        SinPersonas.Visibility = _personas.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

    private void MostrarError(string mensaje)
    {
        ErrorTexto.Text = mensaje;
        ErrorTexto.Visibility = Visibility.Visible;
    }

    private void OcultarError() => ErrorTexto.Visibility = Visibility.Collapsed;
}
