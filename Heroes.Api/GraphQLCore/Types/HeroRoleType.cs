using GraphQL.Types;

namespace Heroes.Api.GraphQLCore.Types
{
	public class HeroRoleType : EnumerationGraphType
	{
		public HeroRoleType()
		{
			Name = "HeroRoleType";
			Description = "Role types for heroes.";
			AddValue("Assassin", "self descriptive", "assassin");
			AddValue("Fighter", "self descriptive", "fighter");
			AddValue("Mage", "self descriptive", "mage");
			AddValue("Support", "self descriptive", "support");
			AddValue("Tank", "self descriptive", "tank");
			AddValue("Marksman", "self descriptive", "marksman");
		}
	}
}