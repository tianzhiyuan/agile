using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Agile.Common.Serialization
{
    public class Json : IJsonSerializer
    {
        private readonly JsonSerializer _serializer;
        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        };
        public Json()
        {
            _serializer = new JsonSerializer()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            };
        }
        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, _settings);
        }

        public TData Deserialize<TData>(string data)
        {
            using (var reader = new JsonTextReader(new StringReader(data)))
            {
                return this._serializer.Deserialize<TData>(reader);
            }
        }

        public object Deserialize(string data, Type type)
        {
            using (var reader = new JsonTextReader(new StringReader(data)))
            {
                return this._serializer.Deserialize(reader, type);
            }
        }
    }
}
