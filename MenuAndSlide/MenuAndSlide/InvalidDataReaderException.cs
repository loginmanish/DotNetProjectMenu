using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace MenuAndSlide
{
    public class InvalidDataReaderException : Exception
    {
        public InvalidDataReaderException() : base("Invalid reader object") { }

        public InvalidDataReaderException(string message) : base(message) { }

        public InvalidDataReaderException(string message, Exception inner) : base(message, inner) { }
    }
}
