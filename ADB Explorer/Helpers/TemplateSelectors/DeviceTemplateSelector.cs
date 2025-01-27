﻿using ADB_Explorer.ViewModels;

namespace ADB_Explorer.Helpers;

public class DeviceTemplateSelector : DataTemplateSelector
{
    public DataTemplate LogicalDeviceTemplate { get; set; }
    public DataTemplate ServiceDeviceTemplate { get; set; }
    public DataTemplate NewDeviceTemplate { get; set; }
    public DataTemplate HistoryDeviceTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container) => item switch
    {
        LogicalDeviceViewModel => LogicalDeviceTemplate,
        ServiceDeviceViewModel => ServiceDeviceTemplate,
        HistoryDeviceViewModel => HistoryDeviceTemplate,
        NewDeviceViewModel => NewDeviceTemplate,
        _ => throw new NotImplementedException(),
    };
}
