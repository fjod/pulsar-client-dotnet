// See https://aka.ms/new-console-template for more information

using Pulsar.Client.Api;
using Pulsar.Client.Common;

Console.WriteLine("Hello, World!");
int?  test = null;

void Test(int? q)
{
    Console.Write(q.ToString());    
}

Test(test);

string url = "pulsar://127.0.0.1:6650";
        
PulsarClient? client = await new PulsarClientBuilder()
    .ServiceUrl(url)
    .BuildAsync();

IConsumer<byte[]> consumer = await client.NewConsumer(Schema.BYTES())
    .Topic("test")
    .SubscriptionName("test_group")
    .ConsumerName("test")
    .SubscriptionType(Pulsar.Client.Common.SubscriptionType.Shared)
    .SubscriptionInitialPosition(SubscriptionInitialPosition.Earliest)
    .AckTimeout(TimeSpan.FromSeconds(20))
    .SubscribeAsync();

while (true)
{
    try
    {
        using (var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(2)))
        {
            Console.WriteLine("{0} test xx begin",  DateTime.Now);
            var message = await consumer.ReceiveAsync(cts.Token);
            Console.WriteLine("{0} test xx end",  DateTime.Now);
            if (message != null)
            {
                await consumer.AcknowledgeAsync(message.MessageId);
                Console.WriteLine("{0} recv a message",  DateTime.Now);
            }
                    
        }
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("{0} test xx empty",  DateTime.Now);
    }
    catch (Exception e)
    {
        Console.WriteLine("{0} [Error] ReceiveAsync error:{1}",  DateTime.Now, e.ToString());
        break;
    }
}

await client.CloseAsync();