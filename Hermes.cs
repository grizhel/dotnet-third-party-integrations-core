using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Confluent.Kafka;
using dotnet_third_party_integrations_core.kafka.models;

namespace dotnet_third_party_integrations_core.Kafka
{
	public static class Hermes
	{
		public static async Task<int> SendMessage(KafkaOptions conf, string topic, object data)
		{
			return await Task.Run(() =>
			 {
				 using (var p = new ProducerBuilder<Null, string>(conf.GetConfig()).Build())
				 {
					 p.Produce(topic, new Message<Null, string> { Value = JsonSerializer.Serialize(data) });
				 }
				 return 0;
			 });
		}

		public static async Task<object?> Subscribe<T>(KafkaOptions conf, string topic)
		{
			return await Task.Run(() =>
			{
				using (var c = new ConsumerBuilder<Ignore, string>(conf.GetConfig()).Build())
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
						return default;
					}
				}
			});
		}
	}
}
