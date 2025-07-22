using System;

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
    internal sealed class CommandHandler : System.Windows.Input.ICommand
    {
        private readonly string _command;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandler"/> class with the specified command string.
        /// </summary>
        /// <param name="command">The AutoCAD command string to be executed.</param>
        public CommandHandler(string command) => _command = command;

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
            Document document = Application.DocumentManager.MdiActiveDocument;
            if (document != null)
            {
                // Sends the command to AutoCAD for execution in the command line
                document.SendStringToExecute(_command + " ", true, false, false);
            }
        }
    }
}