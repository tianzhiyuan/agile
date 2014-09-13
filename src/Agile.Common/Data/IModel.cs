using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Common.Data
{
    public interface IModel
    {
        int? ID { get; set; }
        int? CreatorID { get; set; }
        int? LastModifierID { get; set; }
        DateTime? CreatedAt { get; set; }
        DateTime? LastModifiedAt { get; set; }
    }
}
