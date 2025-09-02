using Shin_Megami_Tensei_View;

namespace Shin_Megami_Tensei
{
    public class TeamFileSelector
    {
        private const string TEAM_SELECTION_MESSAGE = "Elige un archivo para cargar los equipos";
        private const string FILE_OPTION_FORMAT = "{0}: {1}";
        
        private readonly View view;

        public TeamFileSelector(View view)
        {
            this.view = view;
        }

        public void ShowTeamFiles(string[] files)
        {
            ShowTeamSelectionHeader();
            ShowFileOptions(files);
        }

        private void ShowTeamSelectionHeader()
        {
            view.WriteLine(TEAM_SELECTION_MESSAGE);
        }

        private void ShowFileOptions(string[] files)
        {
            for (int i = 0; i < files.Length; i++)
            {
                ShowFileOption(i, files[i]);
            }
        }

        private void ShowFileOption(int index, string filePath)
        {
            view.WriteLine(string.Format(FILE_OPTION_FORMAT, index, Path.GetFileName(filePath)));
        }
    }
}
