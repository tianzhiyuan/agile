using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Common.Serialization
{
    public interface IJsonSerializer
    {
        string Serialize(object obj);
        TData Deserialize<TData>(string data);
    }
}
