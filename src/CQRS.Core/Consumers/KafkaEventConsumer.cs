using System.Text.Json;
using System.Text.Json.Serialization;
using Confluent.Kafka;
using CQRS.Core.Events;

namespace CQRS.Core.Consumers;

public class KafkaEventConsumer: IEventConsumer
{
    private readonly ConsumerConfig _config;

    public KafkaEventConsumer(ConsumerConfig config)
    {
        _config = config;
    }

    public Message<K, V> Consume<K, V>(string topic)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<(BaseEvent, Action)> Consume(string topic, JsonConverter converter, CancellationToken ct)
    {
        using var consumer = GetConsumer();
        consumer.Subscribe(topic);

        while(!ct.IsCancellationRequested)
        {
            var consumeResult = consumer.Consume();

            if(consumeResult?.Message == null) continue;

            var options = new JsonSerializerOptions{ Converters = { converter }};
            var @event = JsonSerializer.Deserialize<BaseEvent>(consumeResult.Message.Value, options);
            
            if(@event == null) throw new NullReferenceException("Evento não convertido...");

            Action commit = () => consumer.Commit(consumeResult);

            yield return (@event, commit);
        }        
    }    

    private IConsumer<string, string> GetConsumer()
    {
        return new ConsumerBuilder<string, string>(_config)
            .SetKeyDeserializer(Deserializers.Utf8)
            .SetValueDeserializer(Deserializers.Utf8)
            .Build();
    }
}

