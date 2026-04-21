using Heroes.Server.Gql.Types;
using Heroes.Server.Sample;

namespace Heroes.Server.Gql;

public class AppGraphSubscription : ObjectGraphType
{
	public AppGraphSubscription(
		IHeroService service
	)
	{
		Field<ListGraphType<HeroGraphType>>("heroAdded")
			.Description("hero added")
			.Resolve(_ => service.Heroes())
			.ResolveStream(_ => service.AddedHero());
	}
}