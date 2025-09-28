using System.IO;
using System.Linq;
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
            pathResolver = new TeamPathResolver();
            fileSelector = new TeamFileSelector(view);
        }

        public void InitializeTeamsPath(string teamsPath)
        {
            pathResolver.InitializeTeamsPath(teamsPath);
        }

        public string? GetTeamsFile()
        {
            var specificFile = pathResolver.GetSpecificFile();
            if (!string.IsNullOrWhiteSpace(specificFile))
            {
                return specificFile;
            }

            return SelectFileFromUser();
        }

        private string? SelectFileFromUser()
        {
            var files = LoadTeamsInOrder();
            if (files.Length == 0)
            {
                return null;
            }

            fileSelector.ShowTeamFiles(files);
            return ReadSelection(files);
        }

        private string[] LoadTeamsInOrder()
        {
            return Directory
                .GetFiles(pathResolver.GetTeamsFolder(), TEXT_FILE_EXTENSION)
                .OrderBy(filePath => filePath)
                .ToArray();
        }

        private string? ReadSelection(string[] files)
        {
            var input = view.ReadLine();
            if (!TryParseFileIndex(input, files.Length, out var index))
            {
                return null;
            }

            return files[index];
        }

        private static bool TryParseFileIndex(string? input, int filesLength, out int index)
        {
            if (!int.TryParse(input, out index))
            {
                return false;
            }

            return index >= MINIMUM_INDEX && index < filesLength;
        }
    }
}
