using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Network
{
    public enum ServerStatus : byte
    {
        //0 - 0     -> Undefined
        Undefined,
        //1 - 9     -> System Information
        Ok,
        Closed,
        //10 - 19   -> Reserved for later usage
        //20 - 29   -> Game Information
        Started = 21,
        Waiting = 22,
        Stopped = 23,
        Paused = 24,
        Ended = 25,
        //30 - 39   -> Player Information
        Joined = 30,
        Left = 31,
        Kicked = 32,
        Banned = 33,
        ConnectionRefused = 34,
        //40 - 49   -> Reserved for later usage
        //50 - 59   -> Errors
        Error = 50,
        NetworkConnectionError = 51,
        Timeout = 52,
    }
}
