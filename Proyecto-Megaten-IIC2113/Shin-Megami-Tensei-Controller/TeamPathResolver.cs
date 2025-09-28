using System;
using System.IO;

namespace Shin_Megami_Tensei
{
    public class TeamPathResolver
    {
        private const string TEXT_FILE_EXTENSION = ".txt";

        private string teamsFolder = string.Empty;
        private string? specificTeamsFile;

        public void InitializeTeamsPath(string teamsPath)
        {
            if (IsSpecificFile(teamsPath))
            {
                SetSpecificFileAndFolder(teamsPath);
                return;
            }

            teamsFolder = teamsPath;
        }

        public string? GetSpecificFile()
        {
            return specificTeamsFile;
        }

        public string GetTeamsFolder()
        {
            return teamsFolder;
        }

        private static bool IsSpecificFile(string teamsPath)
        {
            return teamsPath.EndsWith(TEXT_FILE_EXTENSION, StringComparison.OrdinalIgnoreCase);
        }

        private void SetSpecificFileAndFolder(string teamsPath)
        {
            specificTeamsFile = teamsPath;
            teamsFolder = Path.GetDirectoryName(teamsPath) ?? string.Empty;
        }
    }
}
