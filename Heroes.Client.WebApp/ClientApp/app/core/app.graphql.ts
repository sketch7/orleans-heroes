import { ApolloClient } from "apollo-client";
import { ApolloModule, Apollo } from "apollo-angular";
import { ApolloLink } from "apollo-link";
// import { HttpLink } from "apollo-link-http";
import { HttpLink } from "apollo-angular-link-http";
import { Injectable } from "@angular/core";
import { applyMiddleware } from "redux";
import { InMemoryCache } from "apollo-cache-inmemory";

import { Dictionary } from "../shared/utils";
import { HttpHeaders } from "@angular/common/http";

// const httpLink: HttpLink = new HttpLink({
// 	uri: "http://localhost:62551/graphql",
// 	credentials: "same-origin",
// 	headers: {
// 		"Content-Type": "application/json"
// 	}
// });

// // const wsLink = new WebSocketLink({ uri: process.env.REACT_APP_WS_ROOT });
// const link: ApolloLink = ApolloLink.from([httpLink]);

// export const client: ApolloClient = new ApolloClient({
// 	networkInterface: link,
// 	ssrMode: true
// 	// dataIdFromObject: (o: any) => o["id"],
// 	// enable Apollo Dev Tools Extension
// 	// connectToDevTools: true
// });

// export function getCommonClient(): ApolloClientMap {
// 	return { shared: client };
// }

@Injectable()
export class AppApolloClient {

	// private apollo: ApolloBase;
	// private networkInterface: ApolloLink;

	constructor(
		apollo: Apollo,
		httpLink: HttpLink
	) {

		apollo.create({
			link: httpLink.create({
				uri: "http://localhost:62551/graphql",
				headers: new HttpHeaders({ "Content-Type": "application/json" }),
				withCredentials: true
			}),
			cache: new InMemoryCache()
		});
		// apollo.getClient()
		// this.apollo = apollo.use("shared");
		// this.networkInterface = this.buildNetworkInterface("baseUri");
		// this.configure(this.networkInterface);
	}

	// get(): ApolloBase {
	// 	return this.apollo;
	// }

	// getClient(): ApolloClient {
	// 	return this.apollo.getClient();
	// }

	// getMiddleware(): any {
	// 	return applyMiddleware(this.getClient().middleware());
	// }

	// configure(network: ApolloLink): void {
	// 	// this.getClient().networkInterface = network;
	// }

	// private buildNetworkInterface(apiBaseUri: string): ApolloLink {
	// 	const httpLink: HttpLink = this.buildHttpLink(apiBaseUri);
	// 	const link: ApolloLink = ApolloLink.from([httpLink]);
	// 	return ApolloLink.from([httpLink]);
	// }

	// private buildHttpLink(apiUri: string): HttpLink {
	// 	return new HttpLink({
	// 		uri: "http://localhost:62551/graphql",
	// 		credentials: "same-origin",
	// 		headers: {
	// 			"Content-Type": "application/json"
	// 		}
	// 	});
	// }


	// aPOLLO_MODULE: any[] = [
	// 	apolloModule.forRoot(provideClient)
	// ];
}

// export interface ApolloClientMap extends Dictionary<ApolloClient> {
// }