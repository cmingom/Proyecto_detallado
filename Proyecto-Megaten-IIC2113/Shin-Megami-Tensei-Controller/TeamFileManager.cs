using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Shin_Megami_Tensei_View;

namespace Shin_Megami_Tensei
{
    public class TeamFileManager
    {
        private readonly View view;
        private readonly TeamPathManager teamPathManager;
        private readonly TeamFileDisplay teamFileDisplay;

        public TeamFileManager(View view)
        {
            this.view = view;
            this.teamPathManager = new TeamPathManager();
            this.teamFileDisplay = new TeamFileDisplay(view);
        }

        public void InitializeTeamsPath(string teamsPath)
        {
            teamPathManager.InitializeTeamsPath(teamsPath);
        }

        public string GetTeamsFile()
        {
            if (teamPathManager.HasSpecificFile())
            {
                return teamPathManager.GetSpecificFile();
            }
            
            return GetFileFromUserSelection();
        }

        private string GetFileFromUserSelection()
        {
            var files = GetTeamFiles();
            teamFileDisplay.ShowTeamFiles(files);
            
            if (!TrySelectFile(files, out string selectedFile))
            {
                return string.Empty;
            }
            
            return selectedFile;
        }

        private string[] GetTeamFiles()
        {
            return Directory.GetFiles(teamPathManager.GetTeamsFolder(), "*.txt").OrderBy(f => f).ToArray();
        }

        private bool TrySelectFile(string[] files, out string selectedFile)
        {
            selectedFile = string.Empty;
            var input = GetUserInput();
            if (!IsValidFileIndex(input, files.Length))
            {
                return false;
            }
            selectedFile = GetSelectedFile(files, input);
            return true;
        }

        private string GetUserInput()
        {
            return view.ReadLine();
        }

        private string GetSelectedFile(string[] files, string input)
        {
            return files[int.Parse(input)];
        }
        
        private bool IsValidFileIndex(string input, int filesLength)
        {
            return int.TryParse(input, out int index) && index >= 0 && index < filesLength;
        }
    }
}
