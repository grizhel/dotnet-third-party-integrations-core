﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;

using Confluent.Kafka;
using Grizhla.ThirdPartyIntegrationsCore.kafka.models;
using Grizhla.UtilitiesCore.Helpers.JsonUtilities;


namespace Grizhla.ThirdPartyIntegrationsCore.Kafka;

public static class Hermes
{
	public static async Task SendMessageAsync(KafkaOptions conf, string topic, object data)
	{
		using (var p = new ProducerBuilder<Null, string>(conf.GetConfig()).Build())
		{
			await p.ProduceAsync(topic, new Message<Null, string>
			{
				Value = JsonSerializer.Serialize(data, JsonSerializerUtility.GetJsonSerializerOptions())
			});
		}
	}

	public static async Task SubscribeAsync(KafkaOptions conf, string topic, Func<string, Task> act)
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
						await act(cr.Message.Value);
					}
					catch (ConsumeException e)
					{
						Console.WriteLine($"Error occured: {e.Error.Reason}");
					}
					catch (Exception e)
					{
						Console.WriteLine($"Error occured: {e.Message}");
					}
					finally
					{
						Thread.Sleep(2000);
					}
				}
			}
			catch (OperationCanceledException e)
			{
				Console.WriteLine(e.Message);
			}
			finally
			{
				c.Unsubscribe();
				c.Close();
			}
		}
	}

	public static bool IsRunning(KafkaOptions conf)
	{
		return conf.IsRunning();
	}
}
