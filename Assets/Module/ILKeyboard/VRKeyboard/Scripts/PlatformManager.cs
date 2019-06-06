using UnityEngine;
using System.Collections;

public class PlatformManager{
    private static PlatformManager _Instance;

    public static PlatformManager mInstance
    {
        get
        {
            if(_Instance == null)
            {
                _Instance = new PlatformManager();
            }
            return _Instance;
        }
    }

    public enum PLATFORM
    {
        CHINA,
        JAPAN,
        KOREAN,
    }

    public PLATFORM platform = PLATFORM.JAPAN;

    public void PlatSwitch(string channelNo)
    {
        switch (channelNo)
        {
            case "000":
                platform = PLATFORM.CHINA;
                break;
            case "004":
                platform = PLATFORM.JAPAN;
                break;
            case "007":
                platform = PLATFORM.KOREAN;
                break;
            default:
                platform = PLATFORM.CHINA;
                break;
        }
    }
}
