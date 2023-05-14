using Vdscruz.CQRS.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vdscruz.CQRS.Core.Handlers
{
    public interface IEventHandler<T> where T : BaseEvent
    {
        Task Handler(T @event) ;
    }
}
