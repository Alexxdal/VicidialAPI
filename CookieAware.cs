using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace Emmerre_Admin
{
    /// <summary>
    /// A Cookie-aware WebClient that will store authentication cookie information and persist it through subsequent requests.
    /// </summary>
    public class CookieAwareWebClient : WebClient
    {
        //Properties to handle implementing a timeout
        private int? _timeout = null;
        public int? Timeout
        {
            get
            {
                return _timeout;
            }
            set
            {
                _timeout = value;
            }
        }

        //A CookieContainer class to house the Cookie once it is contained within one of the Requests
        public CookieContainer CookieContainer { get; private set; }

        //Constructor
        public CookieAwareWebClient()
        {
            CookieContainer = new CookieContainer();
        }

        //Method to handle setting the optional timeout (in milliseconds)
        public void SetTimeout(int timeout)
        {
            _timeout = timeout;
        }

        //This handles using and storing the Cookie information as well as managing the Request timeout
        protected override WebRequest GetWebRequest(Uri address)
        {
            ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            //Handles the CookieContainer
            var request = (HttpWebRequest)base.GetWebRequest(address);
            request.CookieContainer = CookieContainer;
            //Sets the Timeout if it exists
            if (_timeout.HasValue)
            {
                request.Timeout = _timeout.Value;
            }
            return request;
        }

        private static bool ValidateRemoteCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error)
        {
            return true;
        }

    }
}
