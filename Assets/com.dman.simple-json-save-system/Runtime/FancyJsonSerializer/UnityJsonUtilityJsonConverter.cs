using System;
using Newtonsoft.Json;

namespace Dman.SimpleJson.FancyJsonSerializer
{
    public class UnityJsonUtilityJsonConverter : JsonConverter
    {
        public override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            var json = UnityEngine.JsonUtility.ToJson(value);
            writer.WriteRawValue(json);
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            if(reader.Value == null)
            {
                return null;
            }
            var json = reader.Value.ToString();
            var result = UnityEngine.JsonUtility.FromJson(json, objectType);
            return result;
        }

        public override bool CanConvert(Type objectType)
        {
            return TypeSet.UnityPrimitiveTypeSet.Contains(objectType);
        }
    }
}