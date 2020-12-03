using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using Task = System.Threading.Tasks.Task;

namespace UprojectShortcuts.Commands
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class SwitchUnrealEngineVersionCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 4131;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("f3b56d74-c40f-4e32-9e5c-42af838fe968");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage Package;

        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchUnrealEngineVersionCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="Package">Owner package, not null.</param>
        /// <param name="CommandService">Command service to add command to, not null.</param>
        private SwitchUnrealEngineVersionCommand(AsyncPackage Package, OleMenuCommandService CommandService)
        {
            this.Package = Package ?? throw new ArgumentNullException(nameof(Package));
            CommandService = CommandService ?? throw new ArgumentNullException(nameof(CommandService));

            CommandID MenuCommandID = new CommandID(CommandSet, CommandId);
            MenuCommand MenuItem = new MenuCommand(this.Execute, MenuCommandID);
            CommandService.AddCommand(MenuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static SwitchUnrealEngineVersionCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.Package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="Package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage Package)
        {
            // Switch to the main thread - the call to AddCommand in ShortcutCommands's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(Package.DisposalToken);

            OleMenuCommandService CommandService = await Package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
            Instance = new SwitchUnrealEngineVersionCommand(Package, CommandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="Sender">Event sender.</param>
        /// <param name="Args">Event args.</param>
        private void Execute(object Sender, EventArgs Args)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            string FilePath = FunctionLibrary.GetCurrentUprojectFilePath();
            FunctionLibrary.InvokeUprojectCommand(
                UprojectCommandType.SwitchUnrealEngineVersion,
                FilePath
            );
        }
    }
}
