using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Magaz.Utility
{
    public static class SessionExtensions
    {

        //  код для обработки сессий хранит целые числа или строки,
        //   этот код для поддержки списков или обектов
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
          var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}
