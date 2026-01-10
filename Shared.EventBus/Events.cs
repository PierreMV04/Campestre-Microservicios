namespace Shared.EventBus;

/// <summary>
/// Interfaz base para todos los eventos del sistema
/// </summary>
public interface IEvent
{
    Guid EventId { get; }
    DateTime Timestamp { get; }
    string EventType { get; }
}

/// <summary>
/// Clase base para eventos
/// </summary>
public abstract class BaseEvent : IEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    public abstract string EventType { get; }
}

/// <summary>
/// Evento de reserva creada
/// </summary>
public class ReservaCreatedEvent : BaseEvent
{
    public override string EventType => "reserva.created";
    public int IdReserva { get; set; }
    public int IdUsuarioExterno { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public List<int> HabitacionesIds { get; set; } = new();
}

/// <summary>
/// Evento de reserva cancelada
/// </summary>
public class ReservaCancelledEvent : BaseEvent
{
    public override string EventType => "reserva.cancelled";
    public int IdReserva { get; set; }
    public string? Motivo { get; set; }
}

/// <summary>
/// Evento de pago realizado
/// </summary>
public class PagoRealizadoEvent : BaseEvent
{
    public override string EventType => "pago.realizado";
    public int IdPago { get; set; }
    public int IdReserva { get; set; }
    public decimal Monto { get; set; }
    public int IdMetodoPago { get; set; }
}

/// <summary>
/// Evento de factura emitida
/// </summary>
public class FacturaEmitidaEvent : BaseEvent
{
    public override string EventType => "factura.emitida";
    public int IdFactura { get; set; }
    public int IdReserva { get; set; }
    public decimal MontoTotal { get; set; }
}

/// <summary>
/// Evento de habitación actualizada
/// </summary>
public class HabitacionUpdatedEvent : BaseEvent
{
    public override string EventType => "habitacion.updated";
    public string IdHabitacion { get; set; } = string.Empty;
    public int IdHotel { get; set; }
    public bool? Disponible { get; set; }
}

/// <summary>
/// Evento de usuario creado
/// </summary>
public class UsuarioCreatedEvent : BaseEvent
{
    public override string EventType => "usuario.created";
    public int IdUsuario { get; set; }
    public string? NombreUsuario { get; set; }
    public string? Email { get; set; }
}
