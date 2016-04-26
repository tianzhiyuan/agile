using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Common.Data
{
    public struct Range<T> where T : struct 
    {
        public T? Left { get; set; }
        public T? Right { get; set; }
    }
    
}
