using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public class NumberJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(int) || objectType == typeof(double) || objectType == typeof(float) || objectType == typeof(decimal)
            || Nullable.GetUnderlyingType(objectType) == typeof(int) || Nullable.GetUnderlyingType(objectType) == typeof(double)
            || Nullable.GetUnderlyingType(objectType) == typeof(float) || Nullable.GetUnderlyingType(objectType) == typeof(decimal);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value is int intValue)
        {
            writer.WriteValue(intValue);
        }
        else if (value is double doubleValue)
        {
            if (doubleValue % 1 == 0)
            {
                writer.WriteValue((int)doubleValue);
            }
            else
            {
                writer.WriteValue(doubleValue);
            }
        }
        else if (value is float floatValue)
        {
            if (floatValue % 1 == 0)
            {
                writer.WriteValue((int)floatValue);
            }
            else
            {
                writer.WriteValue(floatValue);
            }
        }
        else if (value is decimal decimalValue)
        {
            if (decimalValue % 1 == 0)
            {
                writer.WriteValue((int)decimalValue);
            }
            else
            {
                writer.WriteValue(decimalValue);
            }
        }
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return reader.Value;
    }
}

public class IgnorePropertiesResolver : DefaultContractResolver
{
    private readonly HashSet<string> ignoreProps;
    public IgnorePropertiesResolver(IEnumerable<string> propNamesToIgnore)
    {
        ignoreProps = new HashSet<string>(propNamesToIgnore);
    }

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty property = base.CreateProperty(member, memberSerialization);
        if (ignoreProps.Contains(property.PropertyName))
        {
            property.ShouldSerialize = _ => false;
        }
        return property;
    }
}

public class TokenResponse
{
    public string access_token { get; set; }
    public int expires_in { get; set; }
    public string token_type { get; set; }
    public string scope { get; set; }
}
