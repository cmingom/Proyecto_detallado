using System;
using System.IO;

namespace Shin_Megami_Tensei
{
    public class TeamPathResolver
    {
        private string teamsFolder;
        private string? specificTeamsFile;

        public void InitializeTeamsPath(string teamsPath)
        {
            if (IsSpecificFile(teamsPath))
            {
                SetSpecificFileAndFolder(teamsPath);
            }
            else
            {
                SetTeamsFolder(teamsPath);
            }
        }

        private bool IsSpecificFile(string teamsPath)
        {
            return teamsPath.EndsWith(".txt");
        }

        private void SetSpecificFileAndFolder(string teamsPath)
        {
            this.specificTeamsFile = teamsPath;
            this.teamsFolder = Path.GetDirectoryName(teamsPath) ?? "";
        }

        private void SetTeamsFolder(string teamsPath)
        {
            this.teamsFolder = teamsPath;
        }

        public bool HasSpecificFile()
        {
            return specificTeamsFile != null;
        }

        public string GetSpecificFile()
        {
            return specificTeamsFile;
        }

        public string GetTeamsFolder()
        {
            return teamsFolder;
        }
    }
}
