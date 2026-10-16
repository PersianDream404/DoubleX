using MediatR;
using System;
using System.Collections.Generic;
using System.Text;


namespace SharedKernel.Events;

public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken ct)
    where TEvent : IEvent;
}


public interface IEvent : INotification
{
}
public sealed class MediatREventBus(IMediator mediator) : IEventBus
{
    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken ct)
    where TEvent : IEvent
    {
        return mediator.Publish(@event, ct);
    }
}