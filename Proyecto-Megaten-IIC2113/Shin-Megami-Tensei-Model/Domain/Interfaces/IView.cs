namespace Shin_Megami_Tensei_Model.Domain.Interfaces;

public interface IView {
    void WriteLine(string message);
    string? ReadLine();
}