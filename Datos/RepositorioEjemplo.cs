using IdermaGerencia.Models;

namespace IdermaGerencia.Datos;

public static class RepositorioEjemplo
{
    public static readonly DateTime Corte = new(2026, 10, 5);
    public const int CuposDia = 8;

    private static readonly List<Colaborador> _colaboradores = CrearColaboradores();
    private static readonly List<CitaDia> _citas = CrearCitas();

    public static event EventHandler? DatosCambiados;

    public static IReadOnlyList<Colaborador> Colaboradores => _colaboradores;
    public static IReadOnlyList<CitaDia> Citas => _citas.OrderBy(c => c.Inicio).ToList();
    public static IReadOnlyList<Actividad> Actividad { get; } = CrearActividad();
    public static IReadOnlyList<Movimiento> MesCerrado { get; } = CrearMesCerrado();
    public static IReadOnlyList<Movimiento> MesEnCurso { get; } = CrearMesEnCurso();
    public static IReadOnlyList<Insumo> Insumos { get; } = CrearInsumos();
    public static IReadOnlyList<Campana> Campanas { get; } = CrearCampanas();
    public static IReadOnlyList<SedeInfo> Sedes => CrearSedes();

    public static IReadOnlyList<Alerta> AlertasVisibles()
    {
        IEnumerable<Alerta> consulta = ConstruirAlertas();
        if (!ControlesEjemplo.AvisarVencimientos)
            consulta = consulta.Where(a => a.Tipo != "vencimiento");

        return consulta.ToList();
    }

    public static Colaborador? Buscar(string? id) =>
        _colaboradores.FirstOrDefault(c => c.Id == id);

    public static CitaDia? BuscarCita(string? id) =>
        _citas.FirstOrDefault(c => c.Id == id);

    public static Colaborador CrearBorrador(RolClinica rol)
    {
        var esDoctor = rol == RolClinica.Doctor;
        return new Colaborador
        {
            Id = $"{(esDoctor ? "d" : "t")}-{Guid.NewGuid():N}"[..14],
            Nombres = "",
            Apellidos = "",
            Documento = "",
            Correo = "",
            Telefono = "",
            Rol = rol,
            Contrato = new ContratoLaboral
            {
                FechaIngreso = DateTime.Today,
                NumeroContrato = "",
                TipoContrato = esDoctor ? "Locación de servicios" : "Plazo indeterminado",
                Cargo = "",
                Area = esDoctor ? "Área médica" : "",
                Sede = "San Isidro",
                Jornada = esDoctor ? "Por procedimiento" : "Tiempo completo",
                Horario = "",
                SueldoMensual = 0,
                Modalidad = esDoctor ? ModalidadPago.ReciboPorHonorarios : ModalidadPago.Planilla,
                CodigoPago = "",
                Banco = "",
                NumeroCuenta = "",
                AceptaUsoImagen = false,
            },
        };
    }

    public static void GuardarColaborador(Colaborador persona, bool nueva)
    {
        if (nueva)
            _colaboradores.Add(persona);
        else if (_colaboradores.All(c => !ReferenceEquals(c, persona)))
        {
            var indice = _colaboradores.FindIndex(c => c.Id == persona.Id);
            if (indice >= 0)
                _colaboradores[indice] = persona;
            else
                _colaboradores.Add(persona);
        }

        DatosCambiados?.Invoke(null, EventArgs.Empty);
    }

    public static void GuardarCita(CitaDia cita, bool nueva)
    {
        if (string.IsNullOrWhiteSpace(cita.Id))
            cita.Id = Guid.NewGuid().ToString("N");

        if (nueva)
            _citas.Add(cita);
        else
        {
            var indice = _citas.FindIndex(c => c.Id == cita.Id);
            if (indice >= 0)
                _citas[indice] = cita;
            else
                _citas.Add(cita);
        }

        DatosCambiados?.Invoke(null, EventArgs.Empty);
    }

    public static IEnumerable<Colaborador> Activos => Colaboradores.Where(c => c.EstaActivo);

    public static decimal Compromiso(ModalidadPago modalidad) =>
        Activos.Where(c => c.Contrato.Modalidad == modalidad).Sum(c => c.Contrato.SueldoMensual);

    private static List<Colaborador> CrearColaboradores() =>
    [
        Persona(
            "t-lucia", "Lucía", "Fernández Quispe", "DNI 70111221", "lucia.fernandez@iderma.ejemplo", "999 100 201",
            RolClinica.Trabajador, "", "",
            Contrato(new(2022, 3, 12), 2200, "Recepción", "Recepcionista", "San Isidro", ModalidadPago.Planilla, "PL-2026-01",
                true, "Instagram, web y sala de espera", "Renovar antes de usarla en la campaña de cierre de año.",
                "Plazo indeterminado", "Tiempo completo", "Lun a sáb · 8:00 a 17:00", "BCP", "**** 4412", "CT-2022-014",
                "Autorización de imagen vence el 20 de octubre de 2026.",
                consentimiento: new(2025, 10, 20), venceImagen: new(2026, 10, 20))),
        Persona(
            "t-marco", "Marco Antonio", "Ruiz Delgado", "DNI 70883402", "marco.ruiz@iderma.ejemplo", "999 100 202",
            RolClinica.Trabajador, "", "",
            Contrato(new(2023, 1, 4), 2800, "Enfermería", "Enfermero asistencial", "San Isidro", ModalidadPago.Planilla, "PL-2026-02",
                false, "—", "No desea aparecer en redes, web ni material de sala.",
                "Plazo indeterminado", "Por turnos", "Turnos rotativos · 8 horas", "Interbank", "**** 9021", "CT-2023-003",
                "Cobertura de turnos de enfermería en quirófano y recuperación.")),
        Persona(
            "t-camila", "Camila", "Torres Vega", "DNI 71229018", "camila.torres@iderma.ejemplo", "999 100 203",
            RolClinica.Trabajador, "", "",
            Contrato(new(2024, 8, 18), 3400, "Marketing", "Coordinadora de contenidos", "San Isidro", ModalidadPago.ReciboPorHonorarios, "RH-E001-184",
                true, "Redes, web, pauta y vocería", "Puede aparecer como vocera de la clínica.",
                "Locación de servicios", "Medio tiempo", "Lun a vie · 10:00 a 15:00", "BBVA", "**** 1180", "CT-2024-031",
                "Responsable interna de las piezas de marketing. Revisa los consentimientos antes de publicar.",
                consentimiento: new(2025, 8, 18), venceImagen: new(2027, 8, 18))),
        Persona(
            "t-jose", "José Luis", "Paredes Huamán", "DNI 44512098", "jose.paredes@iderma.ejemplo", "999 100 204",
            RolClinica.Trabajador, "", "",
            Contrato(new(2021, 2, 2), 1900, "Mantenimiento", "Auxiliar de mantenimiento", "San Isidro", ModalidadPago.Planilla, "PL-2025-06",
                false, "—", "No autorizó el uso de imagen.",
                "Plazo indeterminado", "Tiempo completo", "Lun a sáb · 7:00 a 16:00", "BCP", "**** 3308", "CT-2021-009",
                "Renuncia voluntaria. La liquidación de ejemplo figura cerrada en junio de 2025.",
                retiro: new(2025, 6, 30), motivoRetiro: "Renuncia voluntaria")),
        Persona(
            "t-andrea", "Andrea", "Salazar Mendoza", "DNI 46770112", "andrea.salazar@iderma.ejemplo", "999 100 205",
            RolClinica.Trabajador, "", "",
            Contrato(new(2020, 11, 9), 4100, "Administración", "Administradora", "San Isidro", ModalidadPago.Planilla, "PL-2026-03",
                true, "Memoria institucional y web interna", "No cubre pauta pagada ni anuncios en redes.",
                "Plazo indeterminado", "Tiempo completo", "Lun a vie · 9:00 a 18:00", "BCP", "**** 2204", "CT-2020-002",
                "Lleva el control de planilla y de recibos por honorarios de la sede.",
                consentimiento: new(2024, 1, 15), venceImagen: new(2027, 1, 15))),
        Persona(
            "t-diego", "Diego", "Navarro Castillo", "DNI 72330045", "diego.navarro@iderma.ejemplo", "999 100 206",
            RolClinica.Trabajador, "", "",
            Contrato(new(2026, 1, 15), 2600, "Coordinación", "Coordinador de pacientes", "Miraflores", ModalidadPago.ReciboPorHonorarios, "RH-E001-203",
                true, "Web y material interno de la sede", "Solo con identificación de la sede Miraflores.",
                "Plazo fijo", "Tiempo completo", "Lun a vie · 9:00 a 18:00", "Pendiente", "Pendiente", "CT-2026-004",
                "La cuenta bancaria sigue pendiente. No incluirlo en transferencias hasta registrarla.",
                finContrato: new(2027, 1, 14), consentimiento: new(2026, 1, 15), venceImagen: new(2027, 1, 14))),
        Persona(
            "t-rosa", "Rosa Elena", "Gutiérrez León", "DNI 40199820", "rosa.gutierrez@iderma.ejemplo", "999 100 207",
            RolClinica.Trabajador, "", "",
            Contrato(new(2019, 5, 21), 1700, "Bioseguridad", "Auxiliar de bioseguridad", "San Isidro", ModalidadPago.Planilla, "PL-2026-04",
                false, "—", "No desea aparecer en redes ni en material público.",
                "Plazo indeterminado", "Tiempo completo", "Lun a sáb · 7:00 a 15:00", "BanBif", "**** 7730", "CT-2019-021",
                "Personal de limpieza clínica y bioseguridad de cabinas.")),
        Persona(
            "d-valeria", "Valeria", "Campos Ríos", "DNI 41992010", "valeria.campos@iderma.ejemplo", "999 200 301",
            RolClinica.Doctor, "CMP 45821 · RNE 31880", "Tricología médica",
            Contrato(new(2018, 4, 3), 12500, "Área médica", "Médica tricóloga", "San Isidro", ModalidadPago.ReciboPorHonorarios, "RH-E001-010",
                true, "Web, charlas, redes y sala de espera", "Vocera médica de la clínica.",
                "Locación de servicios", "Por procedimiento", "Agenda médica · lun a vie", "BCP", "**** 8801", "CT-2018-001",
                "Puede aparecer en charlas y piezas institucionales.",
                consentimiento: new(2025, 4, 3), venceImagen: new(2027, 4, 3))),
        Persona(
            "d-hernan", "Hernán", "Delgado Soto", "DNI 42881033", "hernan.delgado@iderma.ejemplo", "999 200 302",
            RolClinica.Doctor, "CMP 39210 · RNE 27741", "Cirugía capilar",
            Contrato(new(2019, 9, 11), 18000, "Área quirúrgica", "Cirujano capilar", "San Isidro", ModalidadPago.ReciboPorHonorarios, "RH-E001-011",
                true, "Casos clínicos, web y redes", "El rostro del paciente solo se muestra si el paciente también autoriza.",
                "Locación de servicios", "Por procedimiento", "Quirófano · mar, jue y sáb", "Interbank", "**** 6619", "CT-2019-011",
                "Uso de imagen permitido en casos clínicos con el consentimiento del paciente.",
                consentimiento: new(2025, 9, 1), venceImagen: new(2027, 9, 1))),
        Persona(
            "d-mariana", "Mariana", "Ortiz León", "DNI 45501928", "mariana.ortiz@iderma.ejemplo", "999 200 303",
            RolClinica.Doctor, "CMP 51002 · RNE 40112", "Dermatología",
            Contrato(new(2021, 2, 7), 9800, "Área médica", "Dermatóloga", "Miraflores", ModalidadPago.Planilla, "PL-2026-09",
                false, "Credencial interna", "No autoriza redes ni pauta. Solo la credencial de la sede.",
                "Plazo fijo", "Tiempo completo", "Lun a vie · 9:00 a 18:00", "BBVA", "**** 5520", "CT-2021-018",
                "Definir la renovación antes del 15 de noviembre de 2026.",
                finContrato: new(2026, 11, 15))),
        Persona(
            "d-pablo", "Pablo", "Iglesias Marín", "DNI 40221176", "pablo.iglesias@iderma.ejemplo", "999 200 304",
            RolClinica.Doctor, "CMP 28744", "Medicina estética",
            Contrato(new(2020, 3, 1), 8000, "Área médica", "Médico estético", "San Isidro", ModalidadPago.ReciboPorHonorarios, "RH-E001-044",
                true, "Web y redes, solo durante el vínculo", "La autorización no continúa después del retiro.",
                "Locación de servicios", "Por procedimiento", "Agenda cerrada", "BCP", "**** 1044", "CT-2020-016",
                "Vínculo concluido. No programar agenda ni piezas nuevas con su imagen.",
                retiro: new(2024, 12, 31), motivoRetiro: "Fin de vínculo",
                consentimiento: new(2023, 3, 1), venceImagen: new(2024, 12, 31))),
    ];

    private static List<CitaDia> CrearCitas() =>
    [
        new()
        {
            Id = "c1",
            Inicio = new(2026, 10, 5, 8, 30, 0),
            Asunto = "Comité de gerencia",
            Sala = "Sala de reuniones",
            Estado = "Confirmada",
            Personas = ["Andrea Salazar Mendoza", "Valeria Campos Ríos", "Camila Torres Vega"],
        },
        new()
        {
            Id = "c2",
            Inicio = new(2026, 10, 5, 9, 15, 0),
            Asunto = "Revisión de quirófano",
            Sala = "Quirófano",
            Estado = "En curso",
            Personas = ["Hernán Delgado Soto", "Marco Antonio Ruiz Delgado", "Rosa Elena Gutiérrez León"],
        },
        new()
        {
            Id = "c3",
            Inicio = new(2026, 10, 5, 11, 0, 0),
            Asunto = "Coordinación de marketing",
            Sala = "Sala de reuniones",
            Estado = "Confirmada",
            Personas = ["Camila Torres Vega", "Valeria Campos Ríos", "Andrea Salazar Mendoza"],
        },
        new()
        {
            Id = "c4",
            Inicio = new(2026, 10, 5, 15, 30, 0),
            Asunto = "Cierre de planilla",
            Sala = "Administración",
            Estado = "Confirmada",
            Personas = ["Andrea Salazar Mendoza", "Lucía Fernández Quispe"],
        },
        new()
        {
            Id = "c5",
            Inicio = new(2026, 10, 5, 16, 45, 0),
            Asunto = "Seguimiento de Miraflores",
            Sala = "Sede Miraflores",
            Estado = "Pendiente",
            Personas = ["Diego Navarro Castillo", "Mariana Ortiz León"],
        },
    ];

    private static List<Actividad> CrearActividad() =>
    [
        new() { Momento = "Hoy · 08:10", Texto = "Andrea Salazar abrió el comité de gerencia de las 08:30." },
        new() { Momento = "Hoy · 07:40", Texto = "Lucía Fernández abrió recepción en San Isidro." },
        new() { Momento = "Ayer", Texto = "Se registró el recibo RH-E001-203 de Diego Navarro." },
        new() { Momento = "30 set", Texto = "Andrea Salazar dejó lista la planilla de septiembre, todavía como ejemplo." },
    ];

    private static List<Movimiento> CrearMesCerrado()
    {
        var planilla = Compromiso(ModalidadPago.Planilla);
        var honorarios = Compromiso(ModalidadPago.ReciboPorHonorarios);
        return
        [
            new() { Fecha = "30 set 2026", Concepto = "Consultas y controles", Categoria = "Ingresos", Monto = 86400, EsIngreso = true },
            new() { Fecha = "30 set 2026", Concepto = "Procedimientos capilares", Categoria = "Ingresos", Monto = 214000, EsIngreso = true },
            new() { Fecha = "30 set 2026", Concepto = "Planilla del personal activo", Categoria = "Planilla", Monto = planilla, EsIngreso = false },
            new() { Fecha = "30 set 2026", Concepto = "Recibos por honorarios", Categoria = "Honorarios", Monto = honorarios, EsIngreso = false },
            new() { Fecha = "28 set 2026", Concepto = "Insumos clínicos", Categoria = "Insumos", Monto = 28400, EsIngreso = false },
            new() { Fecha = "30 set 2026", Concepto = "Sedes, servicios y mantenimiento", Categoria = "Sede", Monto = 18200, EsIngreso = false },
        ];
    }

    private static List<Movimiento> CrearMesEnCurso() =>
    [
        new() { Fecha = "01–05 oct", Concepto = "Consultas de la primera semana", Categoria = "Ingresos", Monto = 12800, EsIngreso = true },
        new() { Fecha = "01–05 oct", Concepto = "Procedimientos de la primera semana", Categoria = "Ingresos", Monto = 28400, EsIngreso = true },
        new() { Fecha = "03 oct", Concepto = "Reposición parcial de insumos", Categoria = "Insumos", Monto = 6400, EsIngreso = false },
        new() { Fecha = "05 oct", Concepto = "Servicios de sede, parte del mes", Categoria = "Sede", Monto = 3400, EsIngreso = false },
    ];

    private static List<Insumo> CrearInsumos() =>
    [
        new() { Nombre = "Kits de implante FUE", Unidad = "kits", Stock = 6, Minimo = 8, Capacidad = 40 },
        new() { Nombre = "Lidocaína 2%", Unidad = "frascos", Stock = 3, Minimo = 10, Capacidad = 40 },
        new() { Nombre = "Suero fisiológico", Unidad = "unidades", Stock = 48, Minimo = 20, Capacidad = 80 },
        new() { Nombre = "Gasas estériles", Unidad = "paquetes", Stock = 120, Minimo = 40, Capacidad = 200 },
        new() { Nombre = "Shampoo clínico", Unidad = "unidades", Stock = 18, Minimo = 12, Capacidad = 36 },
        new() { Nombre = "Tubos para PRP", Unidad = "unidades", Stock = 22, Minimo = 15, Capacidad = 50 },
    ];

    private static List<Campana> CrearCampanas() =>
    [
        new()
        {
            Nombre = "Antes y después · octubre",
            Canal = "Instagram y web",
            Estado = "En revisión",
            NotaConsentimiento = "Solo usa imagen de Valeria Campos y Hernán Delgado, ambos con consentimiento vigente. El paciente necesita su propia autorización, que este ejemplo no incluye.",
        },
        new()
        {
            Nombre = "Equipo Iderma",
            Canal = "Reels",
            Estado = "Borrador",
            NotaConsentimiento = "Camila Torres está autorizada como vocera. Andrea Salazar no cubre pauta pagada: no incluirla en el anuncio.",
        },
        new()
        {
            Nombre = "Sala de espera San Isidro",
            Canal = "Pantallas internas",
            Estado = "Activa",
            NotaConsentimiento = "No muestra a quien no autorizó el uso de imagen. Marco Ruiz y Rosa Gutiérrez quedan fuera.",
        },
    ];

    private static List<SedeInfo> CrearSedes()
    {
        (string Nombre, string Enfoque, string Horario)[] defs =
        [
            ("San Isidro", "Sede principal · consultas y cirugía", "Lunes a sábado · 8:00 a 19:00"),
            ("Miraflores", "Consultas y seguimiento", "Lunes a viernes · 9:00 a 18:00"),
        ];

        return defs.Select(def =>
        {
            var activos = Activos.Where(c => c.Contrato.Sede == def.Nombre).ToList();
            var trabajadores = activos.Count(c => c.Rol == RolClinica.Trabajador);
            var doctores = activos.Count(c => c.Rol == RolClinica.Doctor);
            return new SedeInfo
            {
                Nombre = def.Nombre,
                Enfoque = def.Enfoque,
                Horario = def.Horario,
                Estado = "Abierta",
                PersonalTexto = $"{trabajadores} trabajadores · {doctores} doctores activos",
            };
        }).ToList();
    }

    private static List<Alerta> ConstruirAlertas()
    {
        var alertas = new List<Alerta>();
        foreach (var persona in Activos)
        {
            if (persona.Contrato.VencimientoImagen is DateTime vence &&
                persona.Contrato.AceptaUsoImagen &&
                vence <= Corte.AddDays(45))
            {
                alertas.Add(new Alerta
                {
                    Tipo = "vencimiento",
                    Severidad = "Alta",
                    Titulo = $"Consentimiento de imagen por vencer · {persona.NombreCompleto}",
                    Detalle = $"Vence el {Formato.FechaLarga(vence)}. Conviene renovarlo antes de usarlo en una pieza nueva.",
                    PersonaId = persona.Id,
                });
            }

            if (persona.Contrato.FechaFinContrato is DateTime fin && fin <= Corte.AddDays(45))
            {
                alertas.Add(new Alerta
                {
                    Tipo = "vencimiento",
                    Severidad = "Alta",
                    Titulo = $"Contrato por vencer · {persona.NombreCompleto}",
                    Detalle = $"El plazo fijo termina el {Formato.FechaLarga(fin)}. Definir renovación, planilla o cierre.",
                    PersonaId = persona.Id,
                });
            }

            if (persona.Contrato.CuentaPendiente)
            {
                alertas.Add(new Alerta
                {
                    Tipo = "dato",
                    Severidad = "Media",
                    Titulo = $"Cuenta bancaria pendiente · {persona.NombreCompleto}",
                    Detalle = "No se puede transferir el pago de este periodo hasta registrar banco y cuenta.",
                    PersonaId = persona.Id,
                });
            }
        }

        foreach (var insumo in Insumos.Where(i => i.Stock < i.Minimo))
        {
            alertas.Add(new Alerta
            {
                Tipo = "stock",
                Severidad = "Media",
                Titulo = $"Stock bajo · {insumo.Nombre}",
                Detalle = insumo.StockTexto,
                Seccion = "inventario",
            });
        }

        var sinImagen = Activos.Where(c => !c.Contrato.AceptaUsoImagen).Select(c => c.NombreCompleto).ToList();
        if (sinImagen.Count > 0)
        {
            alertas.Add(new Alerta
            {
                Tipo = "imagen",
                Severidad = "Info",
                Titulo = $"{sinImagen.Count} personas activas no autorizan su imagen",
                Detalle = string.Join(", ", sinImagen) + ".",
                Seccion = "marketing",
            });
        }

        return alertas;
    }

    private static Colaborador Persona(
        string id,
        string nombres,
        string apellidos,
        string documento,
        string correo,
        string telefono,
        RolClinica rol,
        string colegiatura,
        string especialidad,
        ContratoLaboral contrato) => new()
    {
        Id = id,
        Nombres = nombres,
        Apellidos = apellidos,
        Documento = documento,
        Correo = correo,
        Telefono = telefono,
        Rol = rol,
        Colegiatura = colegiatura,
        Especialidad = especialidad,
        Contrato = contrato,
    };

    private static ContratoLaboral Contrato(
        DateTime ingreso,
        decimal sueldo,
        string area,
        string cargo,
        string sede,
        ModalidadPago modalidad,
        string codigoPago,
        bool imagen,
        string canales,
        string restricciones,
        string tipo,
        string jornada,
        string horario,
        string banco,
        string cuenta,
        string numero,
        string observaciones,
        DateTime? retiro = null,
        string motivoRetiro = "",
        DateTime? finContrato = null,
        DateTime? consentimiento = null,
        DateTime? venceImagen = null) => new()
    {
        FechaIngreso = ingreso,
        FechaRetiro = retiro,
        MotivoRetiro = motivoRetiro,
        NumeroContrato = numero,
        TipoContrato = tipo,
        FechaFinContrato = finContrato,
        Cargo = cargo,
        Area = area,
        Sede = sede,
        Jornada = jornada,
        Horario = horario,
        SueldoMensual = sueldo,
        Modalidad = modalidad,
        CodigoPago = codigoPago,
        Banco = banco,
        NumeroCuenta = cuenta,
        AceptaUsoImagen = imagen,
        FechaConsentimientoImagen = consentimiento,
        VencimientoImagen = venceImagen,
        CanalesImagen = canales,
        RestriccionesImagen = restricciones,
        Observaciones = observaciones,
    };
}
