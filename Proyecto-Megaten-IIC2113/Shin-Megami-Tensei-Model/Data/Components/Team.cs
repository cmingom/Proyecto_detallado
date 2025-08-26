using System.Collections;

namespace Shin_Megami_Tensei_Model.Data.Components;

public class Team : IEnumerable<Unit>
{
    private readonly List<Unit> _units = [];

    public int Count => _units.Count;
    
    public Unit this[Index index] => _units[index];

    public IEnumerator<Unit> GetEnumerator()
    {
        return _units.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void AddUnit(Unit unit)
    {
        _units.Add(unit);
    }

    public void DeleteUnit(Unit unitToDelete)
    {
        _units.RemoveAll(unit => unit == unitToDelete);
    }
    
    public bool HasSamurai()
    {
        return _units.Any(unit => unit.IsSamurai);
    }
    
    public int SamuraiCount()
    {
        return _units.Count(unit => unit.IsSamurai);
    }
    
    public Unit GetSamurai()
    {
        return _units.FirstOrDefault(unit => unit.IsSamurai);
    }
}