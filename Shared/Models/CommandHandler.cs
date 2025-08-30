#pragma warning disable CS0067, CS8600, CS8612, CS8618, CS8767

using System; // Keep for .NET 4.6
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Diagnostics;

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.ZwCAD.ApplicationServices;
#else
using Autodesk.AutoCAD.ApplicationServices;
#endif
#endregion

namespace Shared.Models
{
    /// <summary>
    /// A simple implementation of the <see cref="System.Windows.Input.ICommand"/> interface
    /// that executes a given AutoCAD command string when invoked.
    /// </summary>
    public sealed class CommandHandler : System.Windows.Input.ICommand
    {
        private readonly string _command;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandler"/> class with the specified command string.
        /// </summary>
        /// <param name="command">The AutoCAD command string to be executed.</param>
        public CommandHandler(string command) => _command = command;

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
        /// Executes the stored AutoCAD command by sending it to the active document.
        /// </summary>
        /// <param name="_">Unused parameter.</param>
        public void Execute(object _)
        {
            if (_command.StartsWith("XX:"))
            {
                if (string.IsNullOrEmpty(RPApp.Config.InstallPath) ||
                    !Directory.Exists(RPApp.Config.InstallPath))
                    return;
                string CurrentWorkingDirectory = RPApp.Projector.CurrentWorkingDirectory;
                string CurrentRoute = RPApp.Projector.CurrentRoute;
                if (string.IsNullOrEmpty(CurrentWorkingDirectory) 
                    || !Directory.Exists(CurrentWorkingDirectory))
                    return;
                string command = _command.Substring(3);
                return;
            }

            Document document = Application.DocumentManager.MdiActiveDocument;
            // Sends the command to AutoCAD for execution in the command line
            document?.SendStringToExecute(_command + " ", true, false, false);
        }

        private readonly Regex _tokenRegex =
            new Regex(@"[\""].+?[\""]|[^ ]+", RegexOptions.Compiled);
        private string[] ParseArgs(string tokens) => _tokenRegex.Matches(tokens)
            .Cast<Match>().Select(match => match.Value.Trim('"')).ToArray();
    }
}