using System.Configuration;
using ServiceStack;

namespace Proiect.Broker
{
    class Program
    {
        static void Main(string[] args)
        {
            var licenseKeyText = ConfigurationManager.AppSettings["servicestack:license"];
            Licensing.RegisterLicense(licenseKeyText);

            Broker b1 = new Broker("Broker1");
            Broker b2 = new Broker("Broker2");
        }
    }
}
