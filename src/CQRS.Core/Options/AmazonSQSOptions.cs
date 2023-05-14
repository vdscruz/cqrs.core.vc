

namespace Vdscruz.CQRS.Core.Options;

public class AmazonSQSOptions
{
    public string Region { get; set; } = null!;
    public string IamAccessKey { get; set; } = null!;
    public string IamSecretKey { get; set; } = null!;
}
