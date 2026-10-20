using IdermaGerencia.Datos;
using IdermaGerencia.Models;
using Microsoft.UI.Xaml.Controls;

namespace IdermaGerencia.Pages;

public sealed partial class FinanzasPage : Page
{
    public FinanzasPage()
    {
        InitializeComponent();

        var cerrados = RepositorioEjemplo.MesCerrado;
        var ingresos = cerrados.Where(m => m.EsIngreso).Sum(m => m.Monto);
        var egresos = cerrados.Where(m => !m.EsIngreso).Sum(m => m.Monto);
        IngresosCerrado.Text = Formato.Dinero(ingresos);
        EgresosCerrado.Text = Formato.Dinero(egresos);
        ResultadoCerrado.Text = Formato.Dinero(ingresos - egresos);

        var planilla = RepositorioEjemplo.Activos
            .Where(c => c.Contrato.Modalidad == ModalidadPago.Planilla)
            .OrderBy(c => c.Apellidos)
            .ToList();
        var honorarios = RepositorioEjemplo.Activos
            .Where(c => c.Contrato.Modalidad == ModalidadPago.ReciboPorHonorarios)
            .OrderBy(c => c.Apellidos)
            .ToList();

        ListaPlanilla.ItemsSource = planilla;
        ListaHonorarios.ItemsSource = honorarios;
        TotalPlanilla.Text = $"Compromiso de octubre · {Formato.Dinero(planilla.Sum(c => c.Contrato.SueldoMensual))}";
        TotalHonorarios.Text = $"Compromiso de octubre · {Formato.Dinero(honorarios.Sum(c => c.Contrato.SueldoMensual))}";

        var retirados = RepositorioEjemplo.Colaboradores.Where(c => !c.EstaActivo).Select(c => c.NombreCompleto);
        NotaRetirados.Text = "Fuera del mes: " + string.Join(" y ", retirados) + ".";

        ListaCerrado.ItemsSource = cerrados;
        ListaCurso.ItemsSource = RepositorioEjemplo.MesEnCurso;
    }
}
