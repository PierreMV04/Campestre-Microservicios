using Microsoft.Extensions.DependencyInjection;
using Shared.EventBus;
using Shared.Data;
using Shared.DTOs;

namespace UsuariosPagosService.Consumers;

public class PdfsEventsConsumer
{
    private readonly IEventBus _eventBus;
    private readonly IServiceScopeFactory _scopeFactory;

    public PdfsEventsConsumer(
        IEventBus eventBus,
        IServiceScopeFactory scopeFactory)
    {
        _eventBus = eventBus;
        _scopeFactory = scopeFactory;
    }

    public void Subscribe()
    {
        _eventBus.Subscribe<FacturaEmitidaEvent>(async evt =>
        {
            // 👇 CREAR SCOPE MANUAL
            using var scope = _scopeFactory.CreateScope();
            var pdfRepository = scope.ServiceProvider
                .GetRequiredService<PdfRepository>();

            Console.WriteLine(
                $"[PdfsEventsConsumer] Factura emitida recibida. FacturaId={evt.IdFactura}");

            var pdf = new PdfDto
            {
                IdFactura = evt.IdFactura,
            };

            await pdfRepository.CrearAsync(pdf);
        });
    }
}