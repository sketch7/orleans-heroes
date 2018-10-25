using System.Collections.Generic;

namespace Heroes.Contracts.Grains
{
	public interface ITenantRegistry<out TTenant>
		where TTenant : class, ITenant
	{
		TTenant Get(string tenant);
		IEnumerable<TTenant> GetAll();
	}
}