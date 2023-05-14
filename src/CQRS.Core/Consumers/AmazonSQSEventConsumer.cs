using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Confluent.Kafka;
using Vdscruz.CQRS.Core.Events;
using Vdscruz.CQRS.Core.Options;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Vdscruz.CQRS.Core.Consumers
{
    public class AmazonSQSEventConsumer : IEventConsumer
    {
        private readonly AmazonSQSOptions _config;

        public AmazonSQSEventConsumer(IOptions<AmazonSQSOptions> config)
        {
            _config = config.Value;
        }

        public Message<K, V> Consume<K, V>(string topic)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<(BaseEvent, Action)> Consume(string topic, JsonConverter converter, CancellationToken ct)
        {
            var config = new AmazonSQSConfig { RegionEndpoint = RegionEndpoint.GetBySystemName(_config.Region), ServiceURL = $"https://sqs.{_config.Region}.amazonaws.com" };
            IAmazonSQS sqs = new AmazonSQSClient(_config.IamAccessKey, _config.IamSecretKey, config );
            var queue = sqs.GetQueueUrlAsync(topic).GetAwaiter().GetResult();
            var receiveRequest = new ReceiveMessageRequest
            {
                QueueUrl = queue.QueueUrl, 
            };

            while (!ct.IsCancellationRequested)
            {
                var messageResponse = sqs.ReceiveMessageAsync(receiveRequest, ct).GetAwaiter().GetResult();
                if (messageResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    // TODO: log de erro.
                    continue;
                }

                foreach (var msg in messageResponse.Messages)
                {
                    var options = new JsonSerializerOptions { Converters = { converter } };
                    var @event = JsonSerializer.Deserialize<BaseEvent>(msg.Body, options);

                    if (@event == null) throw new NullReferenceException("Evento não convertido...");

                    Action commit = () => sqs.DeleteMessageAsync(queue.QueueUrl, msg.ReceiptHandle, ct).GetAwaiter().GetResult();

                    yield return (@event, commit);
                }
            }
        }
    }
}
