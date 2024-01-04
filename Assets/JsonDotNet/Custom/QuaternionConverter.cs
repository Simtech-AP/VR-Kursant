using System;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

class QuaternionConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Quaternion);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject jo = JObject.Load(reader);

        var x = (float)jo["x"];
        var y = (float)jo["y"];
        var z = (float)jo["z"];
        var w = (float)jo["w"];

        return new Quaternion(x, y, z, w);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {

        var q = (Quaternion)value;
        JObject jo = new JObject { { "x", q.x }, { "y", q.y }, { "z", q.z }, { "w", q.w } };

        jo.WriteTo(writer);
    }

    public override bool CanWrite => true;

    public override bool CanRead => true;
}