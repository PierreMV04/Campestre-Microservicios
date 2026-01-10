using Shared.EventBus;

namespace UsuariosPagosService.Consumers;

public class FacturasEventsConsumer
{
    private readonly IEventBus _eventBus;

    public FacturasEventsConsumer(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public void Subscribe()
    {
        _eventBus.Subscribe<FacturaEmitidaEvent>(async evt =>
        {
            Console.WriteLine(
                $"[FacturasEventsConsumer] Factura emitida recibida. " +
                $"FacturaId={evt.IdFactura}, ReservaId={evt.IdReserva}, Monto={evt.MontoTotal}");

            // Aquí puedes:
            // - notificar a otros sistemas
            // - publicar otros eventos
            // - disparar workflows
            // - auditar

            await Task.CompletedTask;
        });
    }
}