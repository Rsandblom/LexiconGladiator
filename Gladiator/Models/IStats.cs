namespace Gladiator.Models
{
	public interface IStats
    {
        int statsId { get; set; }
        string Name { get; set; }
        int Str { get; set; }
        int Hp { get; set; }
        int Xp { get; set; }
        int Def { get; set; }
    }
}
