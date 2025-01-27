﻿using ADB_Explorer.Services;
using ADB_Explorer.Models;
using ADB_Explorer.Helpers;

namespace ADB_Explorer.ViewModels;

public class LogicalDeviceViewModel : DeviceViewModel
{
    #region Full properties

    private LogicalDevice device;
    protected new LogicalDevice Device
    {
        get => device;
        set => Set(ref device, value);
    }

    private bool isOpen = false;
    /// <summary>
    /// Device is open for browsing
    /// </summary>
    public bool IsOpen
    {
        get => isOpen;
        private set => Set(ref isOpen, value);
    }

    private byte? androidVersion = null;
    public byte? AndroidVersion
    {
        get => androidVersion;
        private set => Set(ref androidVersion, value);
    }

    public DateTime DiscoverTime { get; private set; }

    #endregion

    #region Read only properties

    public string Name
    {
        get
        {
            // to prevent displaying [Service] for offline connect services acquired via adb devices -l upon first contact
            if (string.IsNullOrEmpty(Device.Name) && DiscoverTime - DateTime.Now < AdbExplorerConst.SERVICE_DISPLAY_DELAY)
                return " ";

            return Device.Name;
        }
    }

    public string BaseID => Type == DeviceType.Service ? ID.Split('.')[0] : ID;

    public string LogicalID => Type == DeviceType.Service && ID.Count(c => c == '-') > 1 ? ID.Split('-')[1] : ID;

    public RootStatus Root => Device.Root;

    public bool AndroidVersionIncompatible => AndroidVersion is not null && AndroidVersion < AdbExplorerConst.MIN_SUPPORTED_ANDROID_VER;

    public override string Tooltip
    {
        get
        {
            string result = "";

            result += Device.Type switch
            {
                DeviceType.Local => "USB",
                DeviceType.Remote => "WiFi",
                DeviceType.Emulator => "Emulator",
                DeviceType.Service => "mDNS Service",
                DeviceType.Sideload => "USB (Recovery)",
                _ => throw new NotImplementedException(),
            };

            result += Device.Status switch
            {
                DeviceStatus.Ok => "",
                DeviceStatus.Offline => " - Offline",
                DeviceStatus.Unauthorized => " - Unauthorized",
                _ => throw new NotImplementedException(),
            };

            return result;
        }
    }

    public Battery Battery => Device.Battery;

    public ObservableList<DriveViewModel> Drives => Device.Drives;

    #endregion

    #region Commands

    public BrowseCommand BrowseCommand { get; private set; }
    public RemoveCommand RemoveCommand { get; private set; }
    public ToggleRootCommand ToggleRootCommand { get; private set; }
    public RebootCommand RebootCommand { get; private set; }
    public BootloaderCommand BootloaderCommand { get; private set; }
    public RecoveryCommand RecoveryCommand { get; private set; }
    public SideloadCommand SideloadCommand { get; private set; }
    public SideloadAutoCommand SideloadAutoCommand { get; private set; }

    #endregion

    public LogicalDeviceViewModel(LogicalDevice device) : base(device)
    {
        Device = device;

        BrowseCommand = new(this);
        RemoveCommand = new(this);
        ToggleRootCommand = new(this);
        RebootCommand = new(this);
        BootloaderCommand = new(this);
        RecoveryCommand = new(this);
        SideloadCommand = new(this);
        SideloadAutoCommand = new(this);

        Data.RuntimeSettings.PropertyChanged += RuntimeSettings_PropertyChanged;
    }

    private void RuntimeSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AppRuntimeSettings.DeviceToOpen))
        {
            IsOpen = Data.RuntimeSettings.DeviceToOpen is not null
                && Data.RuntimeSettings.DeviceToOpen.ID == ID;
        }
    }

    #region Setter functions

    public void SetAndroidVersion(string version)
    {
        if (!IsOpen)
            return;

        if (byte.TryParse(version.Split('.')[0], out byte ver))
            AndroidVersion = ver;
    }

    public void UpdateDevice(LogicalDeviceViewModel other) => UpdateDevice(other.Device);

    public void UpdateDevice(LogicalDevice other)
    {
        Device.Name = other.Name;
        SetStatus(other.Status);
    }

    public void EnableRoot(bool enable) => Device.EnableRoot(enable);

    public bool SetRootStatus(RootStatus status)
    {
        if (Root != status)
        {
            Device.Root = status;

            return true;
        }

        return false;
    }

    public void UpdateBattery() => Device.UpdateBattery();

    public Task<bool> UpdateDrives(IEnumerable<Drive> drives, Dispatcher dispatcher, bool asyncClasify = false) => Device.UpdateDrives(drives, dispatcher, asyncClasify);

    #endregion
}
