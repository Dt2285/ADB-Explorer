﻿using ADB_Explorer.Helpers;
using ADB_Explorer.Models;
using static ADB_Explorer.Models.AdbExplorerConst;

namespace ADB_Explorer.Services;

public class PairingQrClass : INotifyPropertyChanged
{
    public PairingQrClass(SolidColorBrush background, SolidColorBrush foreground) : this()
    {
        Background = background;
        Foreground = foreground;
    }

    public PairingQrClass()
    {
        ServiceName = PAIRING_SERVICE_PREFIX + RandomString.GetUniqueKey(10);
        Password = RandomString.GetUniqueKey(12);
    }

    public string ServiceName { get; private set; }
    public string Password { get; private set; }
    public DrawingImage Image => string.IsNullOrEmpty(PairingString) ? null : QrGenerator.GenerateQR(PairingString, Background, Foreground);
    public string PairingString => WiFiPairingService.CreatePairingString(ServiceName, Password);
    public SolidColorBrush Background { get; set; }
    public SolidColorBrush Foreground { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public class WiFiPairingService
{
    /**
    * Format is "WIFI:T:ADB;S:service;P:password;;" (without the quotes)
    */
    public static string CreatePairingString(string service, string password) 
    {
        return $"WIFI:T:ADB;S:{service};P:{password};;";
    }

    public static IEnumerable<ServiceDevice> GetServices()
    {
        ADBService.ExecuteAdbCommand("mdns", out string services, out _, "services");

        List<ServiceDevice> mdnsServices = new();
        var matches = AdbRegEx.RE_MDNS_SERVICE.Matches(services);
        foreach (Match item in matches)
        {
            var id = item.Groups["ID"].Value;
            var portType = item.Groups["PortType"].Value;
            var ipAddress = item.Groups["IpAddress"].Value;
            var port = item.Groups["Port"].Value;

            if (ipAddress == LOOPBACK_IP)
                continue;

            ServiceDevice service = portType == "pairing"
                ? new PairingService(id, ipAddress, port)
                : new ConnectService(id, ipAddress, port);

            service.MdnsType = id.Contains(PAIRING_SERVICE_PREFIX)
                ? ServiceDevice.ServiceType.QrCode
                : ServiceDevice.ServiceType.PairingCode;

            mdnsServices.Add(service);
        }

        return mdnsServices.Distinct();
    }
}
