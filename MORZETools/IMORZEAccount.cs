using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS
{
    public interface ISMSAccount
    {
        string GetMyAccount();
        bool decodeAsync(byte[] enc, out byte[] dec);
        bool updateSynKey(SMSHash hash, byte[] ext, SMSSyncAlgo sync, byte[] key, byte[] iv, string contact);
        IAddressBook GetAddressBook();
    }
}
