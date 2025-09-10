using System.Threading; // Keep for .NET 4.6

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.ZwCAD.ApplicationServices;
using ZwSoft.ZwCAD.EditorInput;
#else
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
#endif
#endregion

namespace Shared.Extensions
{
    public static class EditorExtensions
    {
        /// <summary>
        /// Writes a message to the AutoCAD command line with a prefix.
        /// </summary>
        /// <param name="editor">
        /// The <see cref="Editor"/> instance this method extends.
        /// </param>
        /// <param name="message">
        /// The message to write. If <c>null</c>, an empty string is written.
        /// </param>
        public static void WriteLine(this Editor editor, string message)
            => ThreadPool.QueueUserWorkItem(_ => editor.WriteMessage($"{message ?? string.Empty}\n"));
    }

    public static class DocumentExtensions
    {
        /// <summary>
        /// Writes a message to the command line of the document’s active <see cref="Editor"/>.
        /// </summary>
        /// <param name="document">
        /// The <see cref="Document"/> instance this method extends.
        /// </param>
        /// <param name="message">
        /// The message to write. If <c>null</c>, an empty string is written.
        /// </param>
        public static void WriteLine(this Document document, string message)
            => ThreadPool.QueueUserWorkItem(_ => document.Editor.WriteMessage($"{message ?? string.Empty}\n"));
    }
}
