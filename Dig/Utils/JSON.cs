using System.IO;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using NodaTime;
using NodaTime.Serialization.JsonNet;

namespace Dig.Utils
{
	public static class JSON
	{
		public static readonly JsonSerializerSettings Settings;
		public static readonly JsonSerializer Serializer;

		static JSON()
		{
			Settings = new JsonSerializerSettings
			{
				ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
				DateFormatHandling = DateFormatHandling.IsoDateFormat,
				DateParseHandling = DateParseHandling.DateTimeOffset,
				DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
				DefaultValueHandling = DefaultValueHandling.Include | DefaultValueHandling.Populate,
				FloatFormatHandling = FloatFormatHandling.String,
				FloatParseHandling = FloatParseHandling.Double,
				Formatting = Formatting.Indented,
				MetadataPropertyHandling = MetadataPropertyHandling.Default,
				MissingMemberHandling = MissingMemberHandling.Ignore,
				NullValueHandling = NullValueHandling.Include,
				ObjectCreationHandling = ObjectCreationHandling.Auto,
				PreserveReferencesHandling = PreserveReferencesHandling.All,
				ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
				StringEscapeHandling = StringEscapeHandling.EscapeNonAscii,
				TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
				TypeNameHandling = TypeNameHandling.Auto
			};

			Settings.Converters.Add(new StringEnumConverter(true));
			Settings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

			Serializer = JsonSerializer.CreateDefault(Settings);
		}

		public static T Load<T>(string path)
		{
			using (var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan))
			using (var reader = new StreamReader(file, Encoding.UTF8))
			using (var json = new JsonTextReader(reader))
			{
				return Serializer.Deserialize<T>(json);
			}
		}
	}
}
