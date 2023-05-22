using OpenTelemetry.Context.Propagation;
using OpenTelemetry;
using System.Diagnostics;
using EasyNetQ;
using EasyNetQ.Internals;
using System.Reflection;

namespace SharedDTOs.Monitoring;

public static class PubSubExtensions
{
    private static readonly TextMapPropagator Propagator = new TraceContextPropagator();
    
    public static Task PublishWithTracingAsync<T>(this IBus con, T message) where T : TracingEventBase
    {
        using var activity = Monitoring.ActivitySource.StartActivity(ActivityKind.Producer);
        activity!.AddTag("exchange.name", GetExchangeName<T>(con));
        var activityContext = activity?.Context ?? Activity.Current?.Context ?? default;
    
        var propagationContext = new PropagationContext(activityContext, Baggage.Current);
        Propagators.DefaultTextMapPropagator.Inject(propagationContext, message, (msg, key, value) =>
        {
            msg.Headers[key] = value;
        });
        return con.PubSub.PublishAsync(message);
    }

    public static Task PublishWithTracingAsync<T>(this IBus con, T message, string topic) where T : TracingEventBase
    {
        using var activity = Monitoring.ActivitySource.StartActivity(ActivityKind.Producer);
        activity!.AddTag("exchange.name", GetExchangeName<T>(con));
        var activityContext = activity?.Context ?? Activity.Current?.Context ?? default;

        var propagationContext = new PropagationContext(activityContext, Baggage.Current);
        Propagators.DefaultTextMapPropagator.Inject(propagationContext, message, (msg, key, value) =>
        {
            msg.Headers[key] = value;
        });
        return con.PubSub.PublishAsync(message, topic);
    }

    public static SubscriptionResult SubscribeWithTracingAsync<T>(this IBus con, string subscriptionId, Action<T> onMessage) where T : TracingEventBase
    {
        return con.PubSub.Subscribe(subscriptionId, (T message) =>
        {
            var parentContext = Propagator.Extract(default, message, (msg, key) =>
            {
                if (message.Headers.TryGetValue(key, out var value))
                {
                    return new[] { value.ToString() };
                }
    
                return Enumerable.Empty<string>();
            });

            using var activity = Monitoring.ActivitySource.StartActivity(ActivityKind.Consumer, parentContext.ActivityContext);
            activity!.AddTag("exchange.name", GetExchangeName<T>(con));
            activity!.AddTag("queue.name", GetQueueName<T>(con, subscriptionId));
            var activityContext = activity?.Context ?? Activity.Current?.Context ?? default;
            onMessage(message);
        });
    }

    public static SubscriptionResult SubscribeWithTracingAsync<T>(this IBus con, string subscriptionId, Action<T> onMessage, string topic) where T : TracingEventBase
    {
        return con.PubSub.Subscribe(subscriptionId, (T message) =>
        {
            var parentContext = Propagator.Extract(default, message, (msg, key) =>
            {
                if (message.Headers.TryGetValue(key, out var value))
                {
                    return new[] { value.ToString() };
                }

                return Enumerable.Empty<string>();
            });

            using var activity = Monitoring.ActivitySource.StartActivity(ActivityKind.Consumer, parentContext.ActivityContext);
            activity!.AddTag("exchange.name", GetExchangeName<T>(con));
            activity!.AddTag("queue.name", GetQueueName<T>(con, subscriptionId));
            var activityContext = activity?.Context ?? Activity.Current?.Context ?? default;
            onMessage(message);
        }, x => x.WithTopic(topic));
    }

    public static string GetExchangeName<T>(IBus con) where T : TracingEventBase
    {
        var advancedBus = con.Advanced;
        var conventions = advancedBus.Container.Resolve<IConventions>();

        var exchange = conventions.ExchangeNamingConvention(typeof(T));

        return exchange;
    }

    public static string GetQueueName<T>(IBus con, string subscriptionId) where T : TracingEventBase
    {
        var advancedBus = con.Advanced;
        var conventions = advancedBus.Container.Resolve<IConventions>();

        var queue = conventions.QueueNamingConvention(typeof(T), subscriptionId);

        return queue;
    }
}

