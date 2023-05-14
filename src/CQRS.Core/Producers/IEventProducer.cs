using Vdscruz.CQRS.Core.Events;

namespace Vdscruz.CQRS.Core.Producers
{
    public interface IEventProducer
    {
        Task ProduceAsync<T>(string topic, T @event) where T : BaseEvent;
    }
}