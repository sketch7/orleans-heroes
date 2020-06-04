using GraphQL.Types;
using Heroes.Contracts.Grains.Heroes;

namespace Heroes.Server.Gql.Types
{
	public class HeroRoleGraphType : EnumerationGraphType
	{
		public HeroRoleGraphType()
		{
			Name = "HeroRoleEnum";
			Description = "Role types for heroes.";

			AddValue("assassin", "self descriptive", (int)HeroRoleType.Assassin);
			AddValue("fighter", "self descriptive", (int)HeroRoleType.Fighter);
			AddValue("mage", "self descriptive", (int)HeroRoleType.Mage);
			AddValue("support", "self descriptive", (int)HeroRoleType.Support);
			AddValue("tank", "self descriptive", (int)HeroRoleType.Tank);
			AddValue("marksman", "self descriptive", (int)HeroRoleType.Marksman);
		}
	}
}