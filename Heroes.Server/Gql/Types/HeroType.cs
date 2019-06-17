﻿using GraphQL.Types;
using Heroes.Contracts.Grains.Heroes;

namespace Heroes.Server.Gql.Types
{
	public class HeroType : ObjectGraphType<Hero>
	{
		public HeroType()
		{
			Name = "Hero";
			Description = "A hero object";

			Field<StringGraphType>("id", resolve: ctx => ctx.Source.Key);
			Field(x => x.Key).Description("unique key of a hero.");
			Field(x => x.Name).Description("self descriptive.");
			Field<HeroRoleEnum>("role", "hero role type.");
		}
	}
}