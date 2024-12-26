using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace dotnet_third_party_integrations_core.Kafka
{
	public static class Hermes
	{
		public static void SendMessage(ProducerConfig conf, string topic, object data)
		{
			using (var p = new ProducerBuilder<Null, string>(conf).Build())
			{
				p.Produce(topic, new Message<Null, string> { Value = JsonSerializer.Serialize(data) });
			}
		}

		public static object Subscribe<T>(ProducerConfig conf, string topic)
		{
			using (var c = new ConsumerBuilder<Ignore, string>(conf).Build())
			{
				c.Subscribe(topic);

				CancellationTokenSource cts = new CancellationTokenSource();
				Console.CancelKeyPress += (_, e) =>
				{
					e.Cancel = true;
					cts.Cancel();
				};

				try
				{
					while (true)
					{
						try
						{
							var cr = c.Consume(cts.Token);
							return JsonSerializer.Deserialize<T>(cr.Message.Value);
						}
						catch (ConsumeException e)
						{
							Console.WriteLine($"Error occured: {e.Error.Reason}");
						}
					}
				}
				catch (OperationCanceledException)
				{
					// Ensure the consumer leaves the group cleanly and final offsets are committed.
					c.Close();
				}
			}
			return null;
		}
	}
}
