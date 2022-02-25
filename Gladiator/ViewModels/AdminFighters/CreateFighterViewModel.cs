namespace Gladiator.ViewModels.AdminFighters
{
	public class CreateFighterViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Str { get; set; }
        public int Hp { get; set; }
        public int Xp { get; set; }
        public int Def { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsOpponent { get; set; }
    }
}
