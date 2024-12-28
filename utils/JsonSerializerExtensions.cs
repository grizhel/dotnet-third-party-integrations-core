using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization.Metadata;
using System.IO.Pipelines;

namespace dotnet_third_party_integrations_core.utils
{
	public static partial class TheThird
	{
		public static partial class JsonSerializer
		{
			public static JsonSerializerOptions GetJsonSerializerOptions(JsonSerializerOptions? options = null)
			{
				options ??= new JsonSerializerOptions();
				options.AllowTrailingCommas = true;
				options.ReferenceHandler = ReferenceHandler.IgnoreCycles;
				return options;
			}
		}
	}
}
