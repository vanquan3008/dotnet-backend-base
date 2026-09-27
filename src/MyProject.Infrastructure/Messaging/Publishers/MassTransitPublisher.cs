using MassTransit;
using MyProject.Application.Common.Interfaces;

namespace MyProject.Infrastructure.Messaging.Publishers;

internal sealed class MassTransitPublisher(IPublishEndpoint publishEndpoint) : IMessagePublisher
{
    public Task PublishAsync<TMessage>(
        TMessage message,
        CancellationToken cancellationToken = default)
        where TMessage : class
        => publishEndpoint.Publish(message, cancellationToken);
}
