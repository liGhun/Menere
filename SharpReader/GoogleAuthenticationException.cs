using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReaderSharp
{

    public class GoogleAuthenticationException : Exception
    {
        public GoogleAuthenticationException() : base(Resources.AuthenticationExceptionMessage) { }
    }

}
