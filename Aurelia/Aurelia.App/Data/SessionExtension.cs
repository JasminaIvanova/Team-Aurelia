﻿using Newtonsoft.Json;
using System.Text.Json;

namespace Aurelia.App.Data
{
    public static class SessionExtension
    {
        public static void Set(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
