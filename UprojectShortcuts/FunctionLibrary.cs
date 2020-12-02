using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Forms;
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
        private static readonly string UnrealProjectFileExtension = "uproject";

        /// <summary>
        /// Unreal Project File subkeys registered in the Windows registry.
        /// </summary>
        private static IDictionary<UprojectCommandType, string> CommandFileNameMap = new IDictionary<UprojectCommandType, string>()
        {
            { UprojectCommandType.LaunchGame, "run" },
            { UprojectCommandType.GenerateVisualStudioProjectFiles, "rungenproj" },
            { UprojectCommandType.SwitchUnrealEngineVersion, "switchversion" }
        };

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
                    "*." + UnrealProjectFileExtension, 
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
            if (!File.Exists(ProjectFilePath))
            {
                MessageBox.Show(
                            ".uproject file not found.\r\n" +
                            "It's not an Unreal Engine project, or the .uproject file isn't in the same directory as the .sln file.",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                            );
                return;
            }

            string Path = $"Unreal.ProjectFile\\shell{ CommandFileNameMap[CommandType] }\\command";
            RegistryKey Key = Registry.ClassesRoot.OpenSubKey(Path);
            if (Key == null)
            {
                MessageBox.Show(
                            "The registry for Unreal Engine project files can't be found.\r\n" +
                            "Maybe you don't have Epic Games Launcher installed, or you've never started Unreal Engine.",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                            );
                return;
            }

            string Command = Key.GetValue("");
            Command = Command.Replace("%1", ProjectFilePath);

            ProcessStartInfo StartInfo = new ProcessStartInfo(Command);
            StartInfo.CreateNoWindow = true;
            StartInfo.UseShellExecute = false;

            Process.Start(StartInfo);
        }
    }
}
