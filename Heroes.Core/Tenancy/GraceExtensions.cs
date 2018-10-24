using System;
using System.Collections.Generic;
using System.Text;
using Grace.Data;
using Heroes.Contracts.Grains;

namespace Heroes.Core.Tenancy
{
	public static class GraceExtensions
	{
		private const string TENANT_KEY = "odin:tenant";

		public static ITenant GetTenantContext(this IExtraDataContainer scopeData)
			=> (ITenant)scopeData.GetExtraData(TENANT_KEY);

		public static TTenant GetTenantContext<TTenant>(this IExtraDataContainer scopeData) where TTenant : ITenant
			=> (TTenant)scopeData.GetTenantContext();

		public static ITenant SetTenantContext(this IExtraDataContainer scopeData, ITenant tenant)
			=> (ITenant)scopeData.SetExtraData(TENANT_KEY, tenant);
	}
}
