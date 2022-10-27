namespace Heroes.Core.Tenancy;

public interface ITenantRegistry<out TTenant>
	where TTenant : class, ITenant
{
	TTenant Get(string tenant);
	IEnumerable<TTenant> GetAll();
}