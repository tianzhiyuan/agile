using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agile.Common.Properties;

namespace Agile.Common.Exceptions
{
    [Serializable]
    public class BusinessException : ApplicationException
    {
        public int Code { get; private set; }

        public BusinessException()
            : base()
        {

        }

        public BusinessException(string msg)
            : base(msg)
        {

        }

        public BusinessException(string msg, Exception innerEx)
            : base(msg, innerEx)
        {

        }

        public BusinessException(int errCode)
        {
            Code = errCode;
        }
        
        public BusinessException(string message, int errCode)
            : base(message)
        {
            Code = errCode;
        }

        public BusinessException(int errCode,string msg, Exception inner) : base(msg, inner)
        {
            Code = errCode;
        }
        public BusinessException(string message, RuleViolatedType code):base(message)
        {
            Code = (int) code;
        }

        public static BusinessException NotSupported(string name = null)
        {
            var msg = string.Format(Resources.RuleViolated_NotSupported, name ?? "");
            var ex = new BusinessException(msg, (int) RuleViolatedType.NotSupported);

            return ex;
        }

        public static BusinessException NotAuthorized(string userName = null)
        {
            var msg = string.Format(Resources.RuleViolated_NotAuthorized, userName ?? "");
            var ex = new BusinessException(msg, (int) RuleViolatedType.NotAuthorizaed);
            return ex;
        }

        public static BusinessException NotAuthenticated()
        {
            var ex = new BusinessException(Resources.RuleViolated_NotAuthenticated,
                                               (int) RuleViolatedType.NotAuthenticated);
            return ex;
        }

        public static BusinessException ArgumentNull(string name = null)
        {
            var msg = string.Format(Resources.RuleViolated_ArgumentNull, name ?? "");
            var ex = new BusinessException(msg, (int) RuleViolatedType.ArgumentNull);
            return ex;
        }

        public static BusinessException Duplicated(string name = null)
        {
            var msg = string.Format(Resources.RuleViolated_Duplicated, name ?? "");
            var ex = new BusinessException(msg, (int) RuleViolatedType.Duplicated);
            return ex;
        }

        public static BusinessException ObjectNotFound(string name = null)
        {
            var msg = string.Format(Resources.RuleViolated_ObjectNotFound, name ?? "");
            var ex = new BusinessException(msg, (int) RuleViolatedType.ObjectNotFound);
            return ex;
        }
    }
}
