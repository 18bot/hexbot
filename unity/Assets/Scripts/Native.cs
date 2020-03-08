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

public static bool FreeLibrary(IntPtr hModule)
{
#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
    return WIN_FreeLibrary(hModule);
#else
    return UNIX_FreeLibrary(hModule);
#endif
}

public static IntPtr LoadLibrary(string directory, string libname)
{
#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
    string path = directory + "/" + libname + ".dll";
    return WIN_LoadLibrary();
#elif (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX)
    string path = directory + "/lib" + libname + ".dylib";
    return UNIX_LoadLibrary(path, 0);
#else
    string path = directory + "/lib" + libname + ".so";
    return UNIX_LoadLibrary(path, 0);
#endif
}

public static IntPtr GetProcAddress(IntPtr hModule, string procedureName)
{
#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
    return WIN_GetProcAddress(hModule, procedureName);
#else
    return UNIX_GetProcAddress(hModule, procedureName);
#endif
}

#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
    [DllImport("kernel32", SetLastError = true, EntryPoint = "FreeLibrary")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool WIN_FreeLibrary(IntPtr hModule);

    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "LoadLibrary")]
    public static extern IntPtr WIN_LoadLibrary(string lpFileName);

    [DllImport("kernel32", EntryPoint = "GetProcAddress")]
    public static extern IntPtr WIN_GetProcAddress(IntPtr hModule, string procedureName);
#else
    [DllImport("libdl", EntryPoint = "dlclose")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UNIX_FreeLibrary(IntPtr hModule);

    [DllImport("libdl", EntryPoint = "dlopen")]
    public static extern IntPtr UNIX_LoadLibrary(String fileName, int flags);

    [DllImport("libdl", EntryPoint = "dlsym")]
    public static extern IntPtr UNIX_GetProcAddress(IntPtr hModule, string procedureName);
#endif


}