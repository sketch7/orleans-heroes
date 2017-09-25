//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using GraphQL;
//using GraphQL.Types;
//using Microsoft.AspNetCore.Mvc;

//namespace Heroes.Api.Controllers
//{
//	// todo: move to Shared?
//	[Route("api/[controller]")]
//	public class GraphQLController : Controller
//	{
//		private readonly IDocumentExecuter _documentExecuter;
//		private readonly ISchema _schema;

//		public GraphQLController(
//			IDocumentExecuter documentExecuter,
//			ISchema schema
//		)
//		{
//			_documentExecuter = documentExecuter;
//			_schema = schema;
//		}

//		[HttpPost]
//		public async Task<ActionResult> Post([FromBody] GraphQLQuery query)
//		{
//			var result = await _documentExecuter.ExecuteAsync(_ =>
//			{
//				_.Schema = _schema;
//				_.Query = query.Query;
//				string vars = query.Variables?.ToString();
//				if (!vars.IsNullOrEmpty())
//					_.Inputs = vars.ToInputs();
//			}).ConfigureAwait(false);

//			return result.ToActionResult(this);
//		}
//	}
//}
