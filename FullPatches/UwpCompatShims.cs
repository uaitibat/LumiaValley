// Shim de perfil .NET 5 -> UWP para o experimento fullport.
// So existem para o codigo original compilar; em tempo de execucao o host e o
// Game1 do port incremental, entao estes caminhos nao sao exercitados.
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Xml;

namespace System
{
    public static class Console
    {
        public static void WriteLine() { }
        public static void WriteLine(object value) { }
        public static void WriteLine(string value) { }
        public static void WriteLine(string format, params object[] args) { }
        public static void WriteLine(int value) { }
        public static void WriteLine(bool value) { }
        public static void Write(object value) { }
        public static void Write(string value) { }
        public static string ReadLine() { return null; }
    }

    public enum PlatformID { Win32NT, Win32Windows, Win32s, Unix, MacOSX }

    public sealed class OperatingSystem
    {
        public PlatformID Platform { get { return PlatformID.Win32NT; } }
        public Version Version { get { return new Version(10, 0, 15063); } }
        public string VersionString { get { return "10.0.15063"; } }
    }

    // UWP nao expoe System.Environment com SpecialFolder/OSVersion/Exit.
    public static class Environment
    {
        public static readonly string NewLine = "\r\n";
        public static int TickCount { get { return 0; } }
        public static bool Is64BitProcess { get { return false; } }
        public static long WorkingSet { get { return 0; } }
        public static string StackTrace { get { return ""; } }
        public static string UserName { get { return ""; } }
        public static OperatingSystem OSVersion { get { return new OperatingSystem(); } }

        public enum SpecialFolder
        {
            ApplicationData,
            LocalApplicationData,
            CommonApplicationData,
            MyDocuments,
            Personal,
            Desktop,
            ProgramFiles,
            System,
            Windows,
            UserProfile
        }

        public static string GetFolderPath(SpecialFolder folder) { return ""; }
        public static string[] GetCommandLineArgs() { return new string[0]; }
        public static string GetEnvironmentVariable(string name) { return null; }
        public static void Exit(int exitCode) { }
        public static void FailFast(string message) { }
    }

    public sealed class AppDomain
    {
        private static readonly AppDomain _current = new AppDomain();
        public static AppDomain CurrentDomain { get { return _current; } }
        public string BaseDirectory { get { return ""; } }
        public Assembly[] GetAssemblies() { return new Assembly[0]; }
        public Assembly GetExecutingAssembly() { return typeof(AppDomain).GetTypeInfo().Assembly; }
        public event UnhandledExceptionEventHandler UnhandledException;
        public void RaiseUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            UnhandledExceptionEventHandler handler = UnhandledException;
            if (handler != null) handler(sender, args);
        }
    }

    // Necessario para os indices 'array[^1]' do codigo original (C# 8+).
    public readonly struct Index
    {
        private readonly int _value;
        public Index(int value, bool fromEnd = false)
        {
            _value = fromEnd ? ~value : value;
        }
        public bool IsFromEnd { get { return _value < 0; } }
        public int Value { get { return _value < 0 ? ~_value : _value; } }
        public int GetOffset(int length)
        {
            return _value < 0 ? length + _value + 1 : _value;
        }
        public static implicit operator Index(int value) { return new Index(value); }
    }

    // UWP nao tem Stream.Close()/BinaryWriter.Close()/XmlWriter.Close().
    // Metodos de extensao substituem as chamadas do codigo original.
    public static class UwpCloseExtensions
    {
        public static void Close(this IO.Stream value) { if (value != null) value.Dispose(); }
        public static void Close(this IO.BinaryWriter value) { if (value != null) value.Dispose(); }
        public static void Close(this IO.StreamReader value) { if (value != null) value.Dispose(); }
        public static void Close(this IO.StreamWriter value) { if (value != null) value.Dispose(); }
        public static void Close(this IO.TextReader value) { if (value != null) value.Dispose(); }
        public static void Close(this IO.TextWriter value) { if (value != null) value.Dispose(); }
        public static void Close(this XmlWriter value) { if (value != null) value.Dispose(); }
        public static void Close(this XmlReader value) { if (value != null) value.Dispose(); }
    }

    public static class UwpCollectionExtensions
    {
        public static ReadOnlyCollection<T> AsReadOnly<T>(this T[] array)
        {
            return new ReadOnlyCollection<T>(array);
        }
    }
}

namespace System.Collections
{
    public static class UwpBitArrayExtensions
    {
        public static void CopyTo(this BitArray bits, Array array, int index)
        {
            for (int i = 0; i < bits.Count; i++)
            {
                array.SetValue(bits[i], index + i);
            }
        }
    }
}

namespace System.Threading
{
    public enum ApartmentState { STA, MTA, Unknown }

    public class Thread
    {
        private static readonly Thread _current = new Thread(null);
        public static Thread CurrentThread { get { return _current; } }

        private readonly ThreadStart _start;
        public Thread(ThreadStart start) { _start = start; }
        public bool IsBackground { get; set; }
        public string Name { get; set; }
        public int ManagedThreadId { get { return 1; } }
        public CultureInfo CurrentCulture { get; set; } = CultureInfo.CurrentCulture;
        public CultureInfo CurrentUICulture { get; set; } = CultureInfo.CurrentUICulture;
        public void SetApartmentState(ApartmentState state) { }
        public void Start() { }
        public void Join() { }
        public static void Sleep(int millisecondsTimeout) { }
    }
}

namespace System.Diagnostics
{
    public sealed class ProcessStartInfo
    {
        public ProcessStartInfo() { }
        public ProcessStartInfo(string fileName, string arguments)
        {
            FileName = fileName;
            Arguments = arguments;
        }
        public string FileName { get; set; }
        public string Arguments { get; set; }
        public string WorkingDirectory { get; set; }
        public string Verb { get; set; }
        public bool UseShellExecute { get; set; }
        public bool CreateNoWindow { get; set; }
        public bool RedirectStandardOutput { get; set; }
    }

    public sealed class Process : IDisposable
    {
        private static readonly Process _current = new Process();
        public static Process GetCurrentProcess() { return _current; }
        public static Process Start(string fileName) { return new Process(); }
        public static Process Start(ProcessStartInfo startInfo) { return new Process(); }
        public ProcessStartInfo StartInfo { get; set; } = new ProcessStartInfo();
        public int Id { get { return 0; } }
        public bool HasExited { get { return true; } }
        public void Kill() { }
        public void Close() { }
        public bool WaitForExit(int milliseconds) { return true; }
        public void WaitForExit() { }
        public void Dispose() { }
    }
}
