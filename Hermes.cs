using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Confluent.Kafka;
using dotnet_third_party_integrations_core.kafka.models;
using static Confluent.Kafka.ConfigPropertyNames;

namespace dotnet_third_party_integrations_core.Kafka
{
	public static class Hermes
	{
		public static async Task SendMessageAsync(KafkaOptions conf, string topic, object data)
		{
			using (var p = new ProducerBuilder<Null, string>(conf.GetConfig()).Build())
			{
				await p.ProduceAsync(topic, new Message<Null, string> { Value = JsonSerializer.Serialize(data) });
			}
		}

		public static void Subscribe(KafkaOptions conf, string topic, Action<object> act)
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
					while (!cts.IsCancellationRequested)
					{
						try
						{
							var cr = c.Consume();
							act(cr.Message.Value);
						}
						catch (ConsumeException e)
						{
							Console.WriteLine($"Error occured: {e.Error.Reason}");
						}
						catch (Exception e)
						{
							Console.WriteLine($"Error occured: {e.Message}");
						}
						Thread.Sleep(2000);
					}
				}
				catch (OperationCanceledException e)
				{
					c.Unsubscribe();
					c.Close();
				}
				finally 
				{
					c.Unsubscribe();
					c.Close();
				}
			}
		}
	}
}
