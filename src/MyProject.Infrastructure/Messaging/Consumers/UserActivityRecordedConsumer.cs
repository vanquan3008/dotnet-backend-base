using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using MyProject.Application.UserActivities.Commands.RecordUserActivity;
using MyProject.Application.UserActivities.IntegrationEvents;

namespace MyProject.Infrastructure.Messaging.Consumers;

public sealed class UserActivityRecordedConsumer(
    ISender sender,
    ILogger<UserActivityRecordedConsumer> logger)
    : IConsumer<UserActivityRecordedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<UserActivityRecordedIntegrationEvent> context)
    {
        var message = context.Message;
        logger.LogInformation(
            "Consuming user activity {ActivityId} for user {UserId}. MessageId: {MessageId}",
            message.ActivityId,
            message.UserId,
            context.MessageId);

        await sender.Send(
            new RecordUserActivityCommand(
                message.UserId,
                message.Type,
                message.Details,
                message.ActivityId,
                message.OccurredAt),
            context.CancellationToken);
    }
}
