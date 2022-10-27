using Heroes.Contracts.Stats;

namespace Heroes.Server.Gql.Types;

public class HeroStatsGraphType : ObjectGraphType<HeroStats>
{
	public HeroStatsGraphType()
	{
		Name = "HeroStats";
		Description = "View all hero stats";

		Field(x => x.HeroId).Description("Hero unique id.");
		Field(x => x.WinRate).Description("hero win rates.");
		Field(x => x.BanRate).Description("hero ban rates.");
		Field(x => x.TotalGames).Description("Total games using this hero.");
	}
}