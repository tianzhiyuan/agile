using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Common.Serialization
{
    /// <summary>
    /// json serializer
    /// </summary>
    public interface IJsonSerializer
    {
        /// <summary>
        /// serialize
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        string Serialize(object obj);
        /// <summary>
        /// deserialize
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        TData Deserialize<TData>(string data);

        /// <summary>
        /// deserialize
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        object Deserialize(string data, Type type);
    }
}
