#pragma warning disable CS0067, CS8600, CS8612, CS8618, CS8767

using System; // Keep for .NET 4.6
using System.IO;
using System.Text.RegularExpressions;
using System.Linq; // Keep for .NET 4.6
using System.Runtime.InteropServices;
using System.Diagnostics;

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using AcApp = ZwSoft.ZwCAD.ApplicationServices;
#else
using AcApp = Autodesk.AutoCAD.ApplicationServices;
#endif
#endregion

namespace Shared.Models
{
    /// <summary>
    /// A simple implementation of the <see cref="System.Windows.Input.ICommand"/> interface
    /// that executes a given CAD command string when invoked.
    /// </summary>
    public sealed class CommandHandler : System.Windows.Input.ICommand
    {
        private readonly string _command;
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandler"/> class with the specified command string.
        /// </summary>
        /// <param name="command">The CAD command string to be executed.</param>
        public CommandHandler(string command) => _command = command; // Keep for .NET 4.6

        public string Command => _command;

        /// <inheritdoc/>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Determines whether the command can execute in its current state.
        /// Always returns <c>true</c> in this implementation.
        /// </summary>
        /// <param name="parameter">Unused parameter.</param>
        /// <returns><c>true</c> to indicate the command can always execute.</returns>
        public bool CanExecute(object _) => true;

        /// <summary>
        /// Executes the stored CAD command by sending it to the active document.
        /// </summary>
        /// <param name="_">Unused parameter.</param>
        public void Execute(object _)
        {
            string command = _command;
            var shouldDetach = _command.StartsWith("XX:");
            if (shouldDetach)
                command = _command.Substring(3);
            if (string.IsNullOrWhiteSpace(command))
                return;
            var (Exec, Args) = Win32Args(command);
            if (string.IsNullOrWhiteSpace(Exec) || string.IsNullOrEmpty(Exec))
            {
                Debug.WriteLine($"[&] {Exec} is not defined");
                return;
            }
            Args = ReplacePlaceholders(Args);
            if (shouldDetach)
            {
                try
                {
                    string InstallPath = RPApp.Config.InstallPath;
                    if (string.IsNullOrWhiteSpace(InstallPath) || !Directory.Exists(InstallPath))
                    {
                        Debug.WriteLine($"[&] {InstallPath} is invalid");
                        return;                 
                    }
                    Exec = Path.Combine(InstallPath, Exec);
                    if (!File.Exists(Exec))
                    {
                        Debug.WriteLine($"[&] {Exec} is invalid");
                        return;
                    }
                    ProcessStartInfo detached = new ProcessStartInfo
                    {
                        FileName = Exec,
                        Arguments = Args,
                        UseShellExecute = true,
                        CreateNoWindow = false,
                        WorkingDirectory = InstallPath,
                    };
                    _ = Process.Start(detached);
                    Debug.WriteLine($"[&] Attempted {Exec} with parameters: '{Args}'");
                } catch { }
                return;
            }
            if (!string.IsNullOrEmpty(Args) && !Args.StartsWith(" "))
                Args = " " + Args;
            var _document = AcApp.Application.DocumentManager.MdiActiveDocument;
            if (_document != null)
                _document.SendStringToExecute($"{Exec}{Args} ", true, false, true);
            Debug.WriteLine($"[&] Attempted {Exec} with parameters: '{Args}'");
        }

        [RPPrivateUseOnly]
        private string ReplacePlaceholders(string _args)
        {
            _args = _args
                .Replace("{WorkingDirectory}", RPApp.Projector.CurrentWorkingDirectory ?? string.Empty)
                .Replace("{Route}", RPApp.Projector.CurrentRoute ?? string.Empty)
                .Replace("{SelectedFile}", RPApp.Projector.CurrentProjectFile?.File ?? string.Empty)
                .Replace("{InstallPath}", RPApp.Config.InstallPath ?? string.Empty);
            return Regex.Replace(_args, @"\s+", " ").Trim();
        }

        #region WIN32_API
        [DllImport("shell32.dll", SetLastError = true)]
        private static extern IntPtr CommandLineToArgvW(
            [MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine,
            out int pNumArgs);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LocalFree(IntPtr hMem);

        private readonly struct Win32ArgsResult
        {
            public string Exec { get; }
            public string Args { get; }

            public Win32ArgsResult(string exec, string args)
            {
                Exec = exec;
                Args = args;
            }
            public override string ToString() => $"{Exec} {Args}";
            public void Deconstruct(out string exec, out string args)
            {
                exec = Exec;
                args = Args;
            }
        }

        [RPPrivateUseOnly]
        private Win32ArgsResult Win32Args(string lpCmdLine)
        {
            if (string.IsNullOrWhiteSpace(lpCmdLine))
                return new Win32ArgsResult(string.Empty, string.Empty);
            IntPtr arguments = CommandLineToArgvW(lpCmdLine, out int pNumArgs);
            if (arguments == IntPtr.Zero)
                return new Win32ArgsResult(string.Empty, string.Empty);
            try
            {
                var result = new string[pNumArgs];
                for (int i = 0; i < pNumArgs; i++)
                {
                    IntPtr pointer = Marshal.ReadIntPtr(arguments, i * IntPtr.Size);
                    result[i] = Marshal.PtrToStringUni(pointer);
                }
                string executable = result.FirstOrDefault() ?? string.Empty;
                return new Win32ArgsResult(executable, string.Join(" ", result.Skip(1)));
            }
            finally
            {
                LocalFree(arguments);
            }
        }
        #endregion
    }
}