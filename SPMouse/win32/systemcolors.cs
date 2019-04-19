using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

public static partial class Win32Util
{

    //https://stackoverflow.com/questions/37774307/c-get-windows-8-1-10-selected-color-theme
    [DllImport("uxtheme.dll", EntryPoint = "#95")]
    public static extern uint GetImmersiveColorFromColorSetEx(uint dwImmersiveColorSet, uint dwImmersiveColorType, bool bIgnoreHighContrast, uint dwHighContrastCacheMode);
    [DllImport("uxtheme.dll", EntryPoint = "#96")]
    public static extern uint GetImmersiveColorTypeFromName(IntPtr pName);
    [DllImport("uxtheme.dll", EntryPoint = "#98")]
    public static extern int GetImmersiveUserColorSetPreference(bool bForceCheckRegistry, bool bSkipCheckOnFail);

    public static System.Drawing.Color GetThemeColor()
    {
        var colorSetEx = GetImmersiveColorFromColorSetEx(
            (uint)GetImmersiveUserColorSetPreference(false, false),
            GetImmersiveColorTypeFromName(Marshal.StringToHGlobalUni("ImmersiveStartSelectionBackground")),
            false, 0);

        var colour = System.Drawing.Color.FromArgb((byte)((0xFF000000 & colorSetEx) >> 24), (byte)(0x000000FF & colorSetEx),
            (byte)((0x0000FF00 & colorSetEx) >> 8), (byte)((0x00FF0000 & colorSetEx) >> 16));

        return colour;
    }


    //https://stackoverflow.com/questions/13660976/get-the-active-color-of-windows-8-automatic-color-theme
    [DllImport("dwmapi.dll", EntryPoint = "#127")]
    public static extern void DwmGetColorizationParameters(ref DWMCOLORIZATIONPARAMS parameters);

    public struct DWMCOLORIZATIONPARAMS
    {
        public uint ColorizationColor,
            ColorizationAfterglow,
            ColorizationColorBalance,
            ColorizationAfterglowBalance,
            ColorizationBlurBalance,
            ColorizationGlassReflectionIntensity,
            ColorizationOpaqueBlend;
    }

}