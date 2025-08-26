

namespace Shin_Megami_Tensei_View.GameConsoleView;

public class LoaderView(View view)
{
    public string[] GetTeamFileContent(string teamsFolder)
    {
        var filePath = SelectFileFromTeamsFolder(teamsFolder);
        return File.ReadAllLines(filePath);
    }

    private string SelectFileFromTeamsFolder(string teamsFolder)
    {
        var files = GetFilesFromTeamsFolder(teamsFolder);
        ShowFileOptions(files);
        var selectedOption = Convert.ToInt32(view.ReadLine());
        return files[selectedOption];
    }

    private static string[] GetFilesFromTeamsFolder(string teamsFolder)
    {
        var files = Directory.GetFiles(teamsFolder);
        Array.Sort(files);
        return files;
    }

    private void ShowFileOptions(string[] files)
    {
        view.WriteLine("Elige un archivo para cargar los equipos");
        for (var i = 0; i < files.Length; i++)
        {
            var fileName = Path.GetFileName(files[i]);
            view.WriteLine($"{i}: {fileName}");
        }
    }

    public void ShowTeamFileNotValid()
    {
        view.WriteLine("Archivo de equipos inválido");
    }
}
