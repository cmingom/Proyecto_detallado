namespace Shin_Megami_Tensei_Model.Domain.Interfaces;

public interface IRandomProvider {
    int Next(int minInclusive, int maxExclusive);
    double NextDouble();
}