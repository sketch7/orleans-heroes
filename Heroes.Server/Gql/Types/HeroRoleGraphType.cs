using Heroes.Contracts.Heroes;

namespace Heroes.Server.Gql.Types;

public class HeroRoleGraphType : EnumerationGraphType
{
	public HeroRoleGraphType()
	{
		Name = "HeroRole";
		Description = "Hero role type.";

		Add("assassin", (int)HeroRoleType.Assassin, "self descriptive");
		Add("fighter", (int)HeroRoleType.Fighter, "self descriptive");
		Add("mage", (int)HeroRoleType.Mage, "self descriptive");
		Add("support", (int)HeroRoleType.Support, "self descriptive");
		Add("tank", (int)HeroRoleType.Tank, "self descriptive");
		Add("marksman", (int)HeroRoleType.Marksman, "self descriptive");
	}
}