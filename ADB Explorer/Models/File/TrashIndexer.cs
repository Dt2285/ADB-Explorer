﻿namespace ADB_Explorer.Models;

public class TrashIndexer : INotifyPropertyChanged
{
    private string recycleName;
    public string RecycleName
    {
        get => recycleName;
        set => Set(ref recycleName, value);
    }

    private string originalPath;
    public string OriginalPath
    {
        get => originalPath;
        set => Set(ref originalPath, value);
    }

    private DateTime? dateModified;
    public DateTime? DateModified
    {
        get => dateModified;
        set => Set(ref dateModified, value);
    }

    public string ModifiedTimeString => DateModified?.ToString(CultureInfo.CurrentCulture.DateTimeFormat);

    public string ParentPath
    {
        get
        {
            int originalIndex = OriginalPath.LastIndexOf('/');
            Index index;
            if (originalIndex == 0)
                index = 1;
            else if (originalIndex < 0)
                index = ^0;
            else
                index = originalIndex;

            return OriginalPath[..index];
        }
    }

    public TrashIndexer(string recycleIndex) : this(recycleIndex.Split('|'))
    { }

    public TrashIndexer(params string[] recycleIndex) : this(recycleIndex[0], recycleIndex[1], recycleIndex[2])
    { }

    public TrashIndexer(string recycleName, string originalPath, string dateModified)
        : this(recycleName, originalPath, DateTime.TryParseExact(dateModified, AdbExplorerConst.ADB_EXPLORER_DATE_FORMAT, null, System.Globalization.DateTimeStyles.None, out var res) ? res : null)
    { }

    public TrashIndexer(string recycleName, string originalPath, DateTime? dateModified)
    {
        RecycleName = recycleName;
        OriginalPath = originalPath;
        DateModified = dateModified;
    }

    public override string ToString()
    {
        return $"{RecycleName}|{OriginalPath}|{DateModified.Value.ToString(AdbExplorerConst.ADB_EXPLORER_DATE_FORMAT)}";
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual bool Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
    {
        if (Equals(storage, value))
        {
            return false;
        }

        storage = value;
        OnPropertyChanged(propertyName);

        return true;
    }

    protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
