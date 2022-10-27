namespace Heroes.Contracts;

public static class OrleansConstants
{
	public const string GrainPersistenceStorage = "Persistence";
	public const string GrainMemoryStorage = "MemoryStore";
	public const string PubSubStore = "PubSubStore";
}

public static class StreamConstants
{
	public static readonly Guid HeroStream = Guid.Parse("673c59fd-f21b-4fbe-a7d1-25a30bad9b15");
	public static readonly Guid UserNotificationStream = Guid.Parse("64531bb5-9f98-426a-8d14-b5ae9eab01c7");
}