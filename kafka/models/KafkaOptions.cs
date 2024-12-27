using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Confluent.Kafka;
using dotnet_third_party_integrations_core.Kafka;

namespace dotnet_third_party_integrations_core.kafka.models;


public class KafkaConfig
{
	public string AutoOffsetReset { get; set; }

	public string EnableAutoCommit { get; set; }

	public string EnableAutoOffsetStore { get; set; }

	public string GroupId { get; set; }

	public string SessionTimeoutMs { get; set; }

	public string StatisticsIntervalMs { get; set; }

	public string AllowAutoCreateTopics { get; set; }
}

public class KafkaOptions
{
	public string BootstrapServers { get; set; }

	public KafkaConfig KafkaConfig { get; set; }

	public IEnumerable<KeyValuePair<string, string>> GetConfig()
	{
		Dictionary<string, string> config = new Dictionary<string, string>
		{
			{ "bootstrap.servers", BootstrapServers },
			{ "auto.offset.reset", KafkaConfig.AutoOffsetReset },
			{ "enable.auto.commit", KafkaConfig.EnableAutoCommit },
			{ "enable.auto.offset.store", KafkaConfig.EnableAutoOffsetStore },
			{ "group.id", KafkaConfig.GroupId },
			{ "session.timeout.ms", KafkaConfig.SessionTimeoutMs },
			{ "statistics.interval.ms", KafkaConfig.StatisticsIntervalMs },
			{"allow.auto.create.topics", KafkaConfig.AllowAutoCreateTopics }
		};
		return config;
	}
}
