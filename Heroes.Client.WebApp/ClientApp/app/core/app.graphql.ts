import { HttpHeaders } from "@angular/common/http";
import { isPlatformServer } from "@angular/common";
import { ApolloClient } from "apollo-client";
import { ApolloModule, Apollo } from "apollo-angular";
import { ApolloLink } from "apollo-link";
// import { HttpLink } from "apollo-link-http";
import { HttpLink, HttpLinkHandler } from "apollo-angular-link-http";
import { Injectable, PLATFORM_ID, Inject } from "@angular/core";
import { applyMiddleware } from "redux";
import { InMemoryCache } from "apollo-cache-inmemory";
import { SubscriptionClient } from "subscriptions-transport-ws";
import { WebSocketLink } from "apollo-link-ws";


import { Dictionary } from "../shared/utils";

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

	uri = "http://localhost:62551/graphql";
	subscriptionsUri = "ws://localhost:62551/graphql";

	constructor(
		apollo: Apollo,
		httpLink: HttpLink,
		@Inject(PLATFORM_ID) platformId: Object
	) {

		const httpLk: HttpLinkHandler = httpLink.create({
			uri: this.uri,
			headers: new HttpHeaders({ "Content-Type": "application/json" }),
			withCredentials: true
		});

		let apolloLinks: ApolloLink[] = [httpLk];
		// todo cla: enable this for WebSockets
		// if (!isPlatformServer(platformId)) {
		// 	const wsLink: WebSocketLink = new WebSocketLink(new SubscriptionClient(this.subscriptionsUri, {
		// 		reconnect: true,
		// 		// connectionParams: {
		// 		//   authToken: localStorage.getItem(GC_AUTH_TOKEN)
		// 		// }
		// 	}));
		// 	apolloLinks.push(wsLink);
		// }

		apollo.create({
			link: ApolloLink.from(apolloLinks),
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