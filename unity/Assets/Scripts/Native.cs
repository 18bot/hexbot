using System;
using System.Runtime.InteropServices;
using UnityEngine;

public static class Native
{
    public static T Invoke<T, T2>(IntPtr library, IntPtr funcPtr, params object[] pars)
    {
        var func = Marshal.GetDelegateForFunctionPointer(funcPtr, typeof(T2));
        return (T)func.DynamicInvoke(pars);
    }

    public static void Invoke<T>(IntPtr library, IntPtr funcPtr, params object[] pars)
    {
        var func = Marshal.GetDelegateForFunctionPointer(funcPtr, typeof(T));
        func.DynamicInvoke(pars);
    }

    [DllImport("kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool FreeLibrary(IntPtr hModule);

    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern IntPtr LoadLibrary(string lpFileName);

    [DllImport("kernel32")]
    public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);
}