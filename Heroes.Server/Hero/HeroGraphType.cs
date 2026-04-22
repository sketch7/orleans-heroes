namespace Heroes.Server.Hero;

public class HeroGraphType : ObjectGraphType<HeroModel>
{
	public HeroGraphType()
	{
		Name = "Hero";
		Description = "A Hero character with unique abilities to play with.";

		Field(x => x.Id).Description("Hero unique key.");
		Field(x => x.Name).Description("Hero name.");
		Field(x => x.Popularity).Description("Hero popularity.");

		Field<HeroRoleGraphType>("role")
			.Description("Hero role type e.g. assassin")
			;

		Field<ListGraphType<StringGraphType>, HashSet<string>>("abilities")
			.Resolve(ctx => ctx.Source.Abilities)
			.Description("Hero abilities.")
			;
	}
}
