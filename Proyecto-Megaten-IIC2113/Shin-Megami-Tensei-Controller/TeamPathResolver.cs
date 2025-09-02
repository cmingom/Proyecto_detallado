namespace Shin_Megami_Tensei
{
    public class TeamPathResolver
    {
        private const string TEXT_FILE_EXTENSION = ".txt";
        private const string EMPTY_STRING = "";
        
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

        private bool IsSpecificFile(string teamsPath)
        {
            return teamsPath.EndsWith(TEXT_FILE_EXTENSION);
        }

        private void SetSpecificFileAndFolder(string teamsPath)
        {
            this.specificTeamsFile = teamsPath;
            this.teamsFolder = Path.GetDirectoryName(teamsPath) ?? EMPTY_STRING;
        }

        private void SetTeamsFolder(string teamsPath)
        {
            this.teamsFolder = teamsPath;
        }
    }
}
