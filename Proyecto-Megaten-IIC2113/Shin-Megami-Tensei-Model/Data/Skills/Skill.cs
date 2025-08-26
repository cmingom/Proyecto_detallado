namespace Shin_Megami_Tensei_Model.Data.Skills;

public class Skill
{
    public string Name { get; }
    public string Type { get; }
    public int Cost { get; }
    public int Power { get; }
    public string Target { get; }
    public int Hits { get; }
    public List<string> Effects { get; }

    public Skill(string name, string type, int cost, int power, string target, int hits, List<string> effects)
    {
        Name = name;
        Type = type;
        Cost = cost;
        Power = power;
        Target = target;
        Hits = hits;
        Effects = effects ?? new List<string>();
    }
}
