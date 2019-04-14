using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS
{
    public enum SMSAsyncAlgo
    {
        None = 0,
        RSA = 0x01
    };
    public enum SMSSyncAlgo
    {
        None = 0,
        DES = 0x01
    };
    public enum SMSHash
    {
        None = 0,
        MD5 = 0x01
    };

}
