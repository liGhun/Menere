using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Menere.ExternalServices
{
    public interface IExternalService
    {
        string service_name { get; }
        string name { get; }
        string description { get; }
        string homepage { get; }
        string icon { get; }
        string settings { get; }

        IExternalService read_settings(string settings);
        void add_new_account();
        void Send(Menere.Model.IItem iitem);
    }
}
