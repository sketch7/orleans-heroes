using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Heroes.Contracts.Grains.Heroes;
using Heroes.Contracts.Grains.Mocks;
using Heroes.Core;

namespace Heroes.Api.Sample
{
	public interface IHeroService
	{
		Hero AddHero();
		IObservable<Hero> AddedHero();
		List<Hero> Heroes();
	}

	public class HeroService : IHeroService
	{
		//private readonly ISubject<Hero> _messageStream = new ReplaySubject<Hero>(1);

		private readonly List<Hero> _heroes = new List<Hero>();

		public HeroService()
		{
			//Observable.Interval(TimeSpan.FromSeconds(5)).Subscribe(x => AddHero());
		}

		public Hero AddHero()
		{
			Console.WriteLine(">>> Hero::Adding Hero");
			var hero = MockDataService.GetHeroes().RandomElement();
			_heroes.Add(hero);
			//_messageStream.OnNext(hero);

			return hero;
		}

		public IObservable<Hero> AddedHero()
		{
			return null;
			//return _messageStream.AsObservable();
		}

		public List<Hero> Heroes()
		{
			return _heroes;
		}
	}
}