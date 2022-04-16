using System;
using System.IO;
using System.Runtime.Serialization;

namespace OurOpenSource
{
    public class PathNotFoundException : IOException
    {
        public PathNotFoundException() : base() {; }
        public PathNotFoundException(string message) : base(message) {; }
        public PathNotFoundException(string message, Exception innerException) : base(message, innerException) {; }
        protected PathNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) {; }
    }
}
