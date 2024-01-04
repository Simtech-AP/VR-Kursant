using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

class InstructionConverter : JsonConverter
{
    public override bool CanWrite
    {
        get { return true; }
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Instruction);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject jo = JObject.Load(reader);

        if (jo["type"].Value<int>() == 0) // text
        {
            return jo.ToObject<TextInstruction>(serializer);
        }
        else if (jo["type"].Value<int>() == 1) // move
        {
            return jo.ToObject<MoveInstruction>(serializer);
        }
        else if (jo["type"].Value<int>() == 2) // delay
        {
            return jo.ToObject<DelayInstruction>(serializer);
        }
        else if (jo["type"].Value<int>() == 3) // usetool
        {
            return jo.ToObject<UseToolInstruction>(serializer);
        }
        else if (jo["type"].Value<int>() == 4) // empty
        {
            return jo.ToObject<EmptyInstruction>(serializer);
        }
        else if (jo["type"].Value<int>() == 5) // digital output
        {
            return jo.ToObject<DigitalInInstruction>(serializer);
        }
        else if (jo["type"].Value<int>() == 6) // if block
        {
            return jo.ToObject<IfBlockInstruction>(serializer);
        }
        else
        {
            return null;
        }
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}

