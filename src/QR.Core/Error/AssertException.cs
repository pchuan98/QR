using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QR.Core.Error;

public class AssertException : Exception
{
    public AssertException(string msg, Exception e) : base(msg, e) { }
    public AssertException() : base("Assert error.") { }
    public AssertException(string msg) : base(msg) { }
}