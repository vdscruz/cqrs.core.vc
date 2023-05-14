using System.Text.Json.Serialization;
using Confluent.Kafka;
using Vdscruz.CQRS.Core.Events;

namespace Vdscruz.CQRS.Core.Consumers
{
    public interface IEventConsumer
    {
        Message<K, V> Consume<K, V>(string topic);

        IEnumerable<(BaseEvent, Action)> Consume(string topic, JsonConverter converter, CancellationToken cancellationToken);
    }
}