using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace FoundryGet.Models
{
    public class SemanticVersionConverter : JsonConverter<SemanticVersioning.Version>
    {
        public override SemanticVersioning.Version ReadJson(
            JsonReader reader,
            Type objectType,
            [AllowNull] SemanticVersioning.Version existingValue,
            bool hasExistingValue,
            JsonSerializer serializer
        )
        {
            var jsonValue = (string)reader.Value;
            return new SemanticVersioning.Version(jsonValue);
        }

        public override void WriteJson(
            JsonWriter writer,
            [AllowNull] SemanticVersioning.Version value,
            JsonSerializer serializer
        )
        {
            writer.WriteValue(value.ToString());
        }
    }
}
