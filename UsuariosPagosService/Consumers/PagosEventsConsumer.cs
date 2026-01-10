using Shared.EventBus;

namespace UsuariosPagosService.Consumers;

public class PagosEventsConsumer
{
    private readonly IEventBus _eventBus;

    public PagosEventsConsumer(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public void Subscribe()
    {
        _eventBus.Subscribe<PagoRealizadoEvent>(async evt =>
        {
            Console.WriteLine(
                $"[PagosEventsConsumer] Pago realizado recibido. " +
                $"PagoId={evt.IdPago}, ReservaId={evt.IdReserva}, Monto={evt.Monto}");

            // Aquí luego puedes:
            // - emitir factura
            // - llamar gRPC a Reservas
            // - publicar FacturaEmitidaEvent
            // - etc.

            await Task.CompletedTask;
        });
    }
}