using Shin_Megami_Tensei_View;

namespace Shin_Megami_Tensei
{
    public class TeamFileCoordinator
    {
        private const int MINIMUM_INDEX = 0;
        private const string TEXT_FILE_EXTENSION = "*.txt";
        
        private readonly View view;
        private readonly TeamPathResolver pathResolver;
        private readonly TeamFileSelector fileSelector;

        public TeamFileCoordinator(View view)
        {
            this.view = view;
            this.pathResolver = new TeamPathResolver();
            this.fileSelector = new TeamFileSelector(view);
        }

        public void InitializeTeamsPath(string teamsPath)
        {
            pathResolver.InitializeTeamsPath(teamsPath);
        }

        public string GetTeamsFile()
        {
            if (pathResolver.HasSpecificFile())
            {
                return pathResolver.GetSpecificFile();
            }
            
            return GetFileFromUserSelection();
        }

        private string GetFileFromUserSelection()
        {
            var files = GetTeamFiles();
            DisplayFilesToUser(files);
            return AttemptFileSelection(files);
        }

        // revisar return
        private string[] GetTeamFiles()
        {
            return Directory.GetFiles(pathResolver.GetTeamsFolder(), TEXT_FILE_EXTENSION).OrderBy(f => f).ToArray();
        }

        private void DisplayFilesToUser(string[] files)
        {
            fileSelector.ShowTeamFiles(files);
        }

        // cambiar nombre a mas descriptivo (si es accion no retorna)
        private string AttemptFileSelection(string[] files)
        {
            var selectedFile = TrySelectFile(files);
            return selectedFile ?? string.Empty;
        }

        // lo mismo que arriba (si retorna que tenga su get)
        // hace dos cosas, dividir
        private string? TrySelectFile(string[] files)
        {
            var input = GetUserInput();
            if (!IsValidFileIndex(input, files.Length))
            {
                return null;
            }
            return GetSelectedFile(files, input);
        }

        private string GetUserInput()
        {
            return view.ReadLine();
        }

        private bool IsValidFileIndex(string input, int filesLength)
        {
            return int.TryParse(input, out int index) && index >= MINIMUM_INDEX && index < filesLength;
        }

        private string GetSelectedFile(string[] files, string input)
        {
            var fileIndex = int.Parse(input);
            return files[fileIndex];
        }
    }
}
