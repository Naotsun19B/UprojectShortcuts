using System.IO;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;

namespace UprojectShortcuts
{
    /// <summary>
    /// Define commands that can be executed from the right-click menu of the .uproject file.
    /// </summary>
    enum UprojectCommandType
    {
        LaunchGame,
        GenerateVisualStudioProjectFiles,
        SwitchUnrealEngineVersion,
    }

    /// <summary>
    /// A class that defines common functions used by this extension.
    /// </summary>
    class FunctionLibrary
    {
        /// <summary>
        /// Unreal Engine project file extension.
        /// </summary>
        public static readonly string UprojectExtension = "uproject";

        /// <summary>
        /// Get the path of a .uproject file that is in the same hierarchy as the currently open .sln file.
        /// Returns string.Empty if no .sln file is currently open.
        /// </summary>
        public static string GetCurrentUprojectFilePath()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            IVsSolution Solution = (IVsSolution)Package.GetGlobalService(typeof(IVsSolution));

            if (Solution.GetSolutionInfo(out string SolutionDirectory, out _, out _) == VSConstants.S_OK)
            {
                string[] Files = Directory.GetFiles(
                    SolutionDirectory, 
                    "*." + UprojectExtension, 
                    SearchOption.TopDirectoryOnly
                );

                if (Files.Length == 1)
                {
                    return Files[0];
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Execute by specifying the command type and the path of the .uproject file.
        /// </summary>
        /// <param name="CommandType">Type of command to execute.</param>
        /// <param name="ProjectFilePath">Full file path of the target .uproject file.</param>
        public static void InvokeUprojectCommand(UprojectCommandType CommandType, string ProjectFilePath)
        {

        }
    }
}
