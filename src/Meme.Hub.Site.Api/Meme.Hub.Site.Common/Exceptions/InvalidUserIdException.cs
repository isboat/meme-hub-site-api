using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Meme.Hub.Site.Common.Exceptions
{
    public class InvalidUserIdException : SystemException
    {
        public InvalidUserIdException()
        {
        }

        public InvalidUserIdException(string? message) : base(message)
        {
        }

        public InvalidUserIdException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidUserIdException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
