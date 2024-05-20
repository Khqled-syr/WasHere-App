using WasHere.ViewModel;

namespace WasHere.Utils;

public class KeyAuthApi
{
public static Api KeyAuthApp = new Api(
    name: "WasHere", // Application Name
    ownerid: "aoGZt2osBA", // Owner ID
    secret: "c53805a449326f47b3c35a80ff601886e2afb946b52d685bbde68731da7a5c54", // Application Secret
    version: "1.0" // Application Version, /*
);


    public DateTime UnixTimeToDateTime(long unixtime)
    {
        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Local);

        try
        {
            dtDateTime = dtDateTime.AddSeconds(unixtime).ToLocalTime();
        }
        catch
        {
            dtDateTime = DateTime.MaxValue;
        }
        return dtDateTime;
    }
}