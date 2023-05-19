using OpenTelemetry.Context.Propagation;
using OpenTelemetry;
using System.Diagnostics;
using EasyNetQ;
using EasyNetQ.Internals;

namespace SharedDTOs.Monitoring;

public static class PubSubExtensions
{
    private static readonly TextMapPropagator Propagator = new TraceContextPropagator();
    
    public static Task PublishWithTracingAsync<T>(this IPubSub con, T message) where T : TracingEventBase
    {
        using var activity = Monitoring.ActivitySource.StartActivity(ActivityKind.Producer);
        var activityContext = activity?.Context ?? Activity.Current?.Context ?? default;
    
        var propagationContext = new PropagationContext(activityContext, Baggage.Current);
        Propagators.DefaultTextMapPropagator.Inject(propagationContext, message, (msg, key, value) =>
        {
            msg.Headers[key] = value;
        });
        return con.PublishAsync(message);
    }

    public static Task PublishWithTracingAsync<T>(this IPubSub con, T message, string topic) where T : TracingEventBase
    {
        using var activity = Monitoring.ActivitySource.StartActivity(ActivityKind.Producer);
        var activityContext = activity?.Context ?? Activity.Current?.Context ?? default;

        var propagationContext = new PropagationContext(activityContext, Baggage.Current);
        Propagators.DefaultTextMapPropagator.Inject(propagationContext, message, (msg, key, value) =>
        {
            msg.Headers[key] = value;
        });
        return con.PublishAsync(message, topic);
    }

    public static AwaitableDisposable<SubscriptionResult> SubscribeWithTracingAsync<T>(this IPubSub con, string subscriptionId, Action<T> onMessage) where T : TracingEventBase
    {
        return con.SubscribeAsync(subscriptionId, (T message) =>
        {
            var parentContext = Propagator.Extract(default, message, (msg, key) =>
            {
                if (message.Headers.TryGetValue(key, out var value))
                {
                    return new[] { value.ToString() };
                }
    
                return Enumerable.Empty<string>();
            });
    
            using var activity = Monitoring.ActivitySource.StartActivity("Received message", ActivityKind.Consumer, parentContext.ActivityContext);
            onMessage(message);
        });
    }

    public static AwaitableDisposable<SubscriptionResult> SubscribeWithTracingAsync<T>(this IPubSub con, string subscriptionId, Action<T> onMessage, string topic) where T : TracingEventBase
    {
        return con.SubscribeAsync(subscriptionId, (T message) =>
        {
            var parentContext = Propagator.Extract(default, message, (msg, key) =>
            {
                if (message.Headers.TryGetValue(key, out var value))
                {
                    return new[] { value.ToString() };
                }

                return Enumerable.Empty<string>();
            });

            using var activity = Monitoring.ActivitySource.StartActivity("Received message", ActivityKind.Consumer, parentContext.ActivityContext);
            onMessage(message);
        }, x => x.WithTopic(topic));
    }
}

