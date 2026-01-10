using Shared.EventBus;

namespace UsuariosPagosService.Consumers;

public class UsuariosEventsConsumer
{
    private readonly IEventBus _eventBus;

    public UsuariosEventsConsumer(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public void Subscribe()
    {
        _eventBus.Subscribe<UsuarioCreatedEvent>(async evt =>
        {
            Console.WriteLine(
                $"[UsuariosPagosService] Usuario creado recibido " +
                $"Id={evt.IdUsuario}, Nombre={evt.NombreUsuario}, Email={evt.Email}");

            // Aquí luego puedes:
            // - crear perfil de pagos
            // - inicializar saldo
            // - enviar correo
            // - sincronizar con otro sistema

            await Task.CompletedTask;
        });
    }
}