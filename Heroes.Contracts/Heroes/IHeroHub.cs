﻿namespace Heroes.Contracts.Heroes;

public interface IHeroHub
{
	Task Send(string message);
	Task Broadcast(Hero hero);
	Task HeroChanged(Hero hero);
	Task StreamUnsubscribe(string methodName, string id);
	Task AddToGroup(string name);
}