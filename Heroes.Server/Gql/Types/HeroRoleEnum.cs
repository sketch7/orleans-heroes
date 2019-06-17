using GraphQL.Types;

namespace Heroes.Server.Gql.Types
{
	public class HeroRoleEnum : EnumerationGraphType
	{
		public HeroRoleEnum()
		{
			Name = "HeroRoleEnum";
			Description = "Role types for heroes.";

			AddValue("Assassin", "self descriptive", 1);
			AddValue("Fighter", "self descriptive", 2);
			AddValue("Mage", "self descriptive", 3);
			AddValue("Support", "self descriptive", 4);
			AddValue("Tank", "self descriptive", 5);
			AddValue("Marksman", "self descriptive", 6);
		}
	}
}