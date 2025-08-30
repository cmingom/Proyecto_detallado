using System;
using System.Collections.Generic;
using System.IO;
using Shin_Megami_Tensei_View;

namespace Shin_Megami_Tensei
{
    public class TeamFileDisplay
    {
        private readonly View view;

        public TeamFileDisplay(View view)
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
            view.WriteLine("Elige un archivo para cargar los equipos");
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
            view.WriteLine($"{index}: {Path.GetFileName(filePath)}");
        }
    }
}
