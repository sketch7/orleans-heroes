using GraphQL;
using GraphQL.Types;
using Heroes.Api.GraphQLCore.Queries;

namespace Heroes.Api.GraphQLCore
{
    public class HeroesAppSchema : Schema
    {
        public HeroesAppSchema(IDependencyResolver resolver)
            : base(resolver)
        {
            Query = resolver.Resolve<HeroesAppGraphQuery>();
        }
    }
}