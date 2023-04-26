using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRS.Core.Options
{
    public class AmazonSQSOptions
    {
        public string Region { get; set; } = null!;
        public string IamAccessKey { get; set; } = null!;
        public string IamSecretKey { get; set; } = null!;
    }
}
