﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS
{
    public interface IMORZEContact
    {
        byte[] EncryptPK(byte[] input);
        bool isHasConfirmEtx();
        ExtKey getInitalData();
        byte XOR(byte vl, int pos);
        string GetAddress();
        bool updateSynKey(SMSHash hash, byte[] ext, SMSSyncAlgo sync, byte[] key, byte[] iv);
        
        /// <summary>
        /// прием входящего сообщения
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="hash"></param>
        /// <param name="hashid"></param>
        /// <param name="ext"></param>
        /// <returns>true - сообщение было успешно декодировано</returns>
        bool PutReciveMessage(byte[] msg, byte[] hash, SMSHash hashid, byte[] ext);
    }
}