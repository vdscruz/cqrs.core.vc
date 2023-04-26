using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using CQRS.Core.Events;
using CQRS.Core.Options;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace CQRS.Core.Producers;

public class AmazonSQSEventProducer : IEventProducer
{
    private readonly AmazonSQSOptions _config;

    public AmazonSQSEventProducer(IOptions<AmazonSQSOptions> config)
    {
        _config = config.Value;
    }

    public async Task ProduceAsync<T>(string topic, T @event) where T : BaseEvent
    {
        var config = new AmazonSQSConfig { 
            RegionEndpoint = RegionEndpoint.GetBySystemName(_config.Region), 
            ServiceURL = $"https://sqs.{_config.Region}.amazonaws.com" 
        
        };
        IAmazonSQS sqs = new AmazonSQSClient(_config.IamAccessKey, _config.IamSecretKey, config);
        var queue = await sqs.GetQueueUrlAsync(topic);
        var request = new SendMessageRequest
        {
            QueueUrl = queue.QueueUrl,
            MessageBody = JsonSerializer.Serialize(@event, @event.GetType())
        };

        await sqs.SendMessageAsync(request);
}
}
