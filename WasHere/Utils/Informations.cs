using System.ComponentModel;
using System.Management;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using WasHere.Utils;

namespace WasHere.ViewModel
{
    public class Informations : INotifyPropertyChanged
    {
        // Old properties
        private string? userName;
        private DateTime? lastLogin;
        private DateTime? createDate;
        private readonly KeyAuthApi Api = new KeyAuthApi();

        public string? UserName
        {
            get => userName;
            set
            {
                userName = value;
                OnPropertyChanged();
            }
        }

        public DateTime? LastLogin
        {
            get => lastLogin;
            set
            {
                lastLogin = value;
                OnPropertyChanged();
            }
        }

        public DateTime? CreateDate
        {
            get => createDate;
            set
            {
                createDate = value;
                OnPropertyChanged();
            }
        }

        // New properties
        private string? windowsVersion;
        private string? publicIpAddress;
        private string? connectionType;
        private Brush? connectionStatusColor;

        public string? WindowsVersion
        {
            get => windowsVersion;
            set
            {
                windowsVersion = value;
                OnPropertyChanged();
            }
        }

        public string? PublicIpAddress
        {
            get { return publicIpAddress; }
            set
            {
                if (publicIpAddress != value)
                {
                    publicIpAddress = value;
                    OnPropertyChanged(nameof(PublicIpAddress));
                }
            }
        }

        public string? ConnectionType
        {
            get => connectionType;
            set { connectionType = value; OnPropertyChanged(); }
        }

        public Brush? ConnectionStatusColor
        {
            get => connectionStatusColor;
            set { connectionStatusColor = value; OnPropertyChanged(); }
        }

        public Informations()
        {
            // Old properties initialization
            UserName = App.user.UserName;
            lastLogin = Api.UnixTimeToDateTime(long.Parse(KeyAuthApi.KeyAuthApp.user_data.lastlogin)) ;
            CreateDate = Api.UnixTimeToDateTime(long.Parse(KeyAuthApi.KeyAuthApp.user_data.createdate));

            // New properties initialization
            WindowsVersion = GetWindowsEdition();
            UpdatePublicIpAddress();
            ConnectionType = GetConnectionType();
            UpdateConnectionStatus();
        }

        public static string? GetWindowsEdition()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            foreach (ManagementObject os in searcher.Get())
            {
                object edition = os["Caption"];
                if (edition != null)
                {
                    return edition.ToString();
                }
            }
            return "Unknown";
        }

        public void UpdatePublicIpAddress()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    PublicIpAddress = client.GetStringAsync("https://api.ipify.org").Result;
                }
            }
            catch (Exception)
            {
                PublicIpAddress = "Error: Unable to retrieve public IP";
            }
        }

        public async Task<string> GetPublicIpAddressAsync()
        {
            string ipAddress;
            using (var client = new HttpClient())
            {
                ipAddress = await client.GetStringAsync("https://api.ipify.org");
            }
            return ipAddress;
        }

        private string? GetConnectionType()
        {
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface networkInterface in interfaces)
            {
                if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                {
                    return "Wi-Fi";
                }
                else if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    return "Ethernet";
                }
            }

            return "Unknown";
        }

        private void UpdateConnectionStatus()
        {
            ConnectionStatusColor = IsNetworkConnected() ? Brushes.Green : Brushes.Red;
        }

        private bool IsNetworkConnected()
        {
            return NetworkInterface.GetIsNetworkAvailable();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
