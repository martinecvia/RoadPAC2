#pragma warning disable CS0067, CS8600, CS8612, CS8618, CS8767

using System; // Keep for .NET 4.6

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.ZwCAD.ApplicationServices;
#else
using Autodesk.AutoCAD.ApplicationServices;
#endif
#endregion

namespace Shared
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
        public bool CanExecute(object parameter) => true;

        /// <summary>
        /// Executes the stored AutoCAD command by sending it to the active document.
        /// </summary>
        /// <param name="parameter">Unused parameter.</param>
        public void Execute(object parameter)
        {
            if (_command.StartsWith("XX:"))
            {
                // RP_RUN()

                return;
            }

            Document document = Application.DocumentManager.MdiActiveDocument;
            // Sends the command to AutoCAD for execution in the command line
            document?.SendStringToExecute(_command + " ", true, false, false);
        }
    }
}