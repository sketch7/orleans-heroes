import { ApolloClient, createNetworkInterface } from "apollo-client";
import { ApolloModule, Apollo, ApolloBase } from "apollo-angular";
import { Injectable } from "@angular/core";
import { Dictionary } from "../shared/utils";
import { HTTPNetworkInterface } from "apollo-client/transport/networkInterface";
import { applyMiddleware } from "redux";

export const client: ApolloClient = new ApolloClient({
	networkInterface: createNetworkInterface({
		uri: "http://localhost:62551/graphql",
		opts: {
			credentials: "same-origin",
			headers: {
				"Content-Type": "application/json"
			}
		}
	}),
	ssrMode: true
	// dataIdFromObject: (o: any) => o["id"],
	// enable Apollo Dev Tools Extension
	// connectToDevTools: true
});

export function getCommonClient(): ApolloClientMap {
	return { shared: client };
}

@Injectable()
export class AppApolloClient {

	private apollo: ApolloBase;
	private networkInterface: HTTPNetworkInterface;

	constructor(apollo: Apollo) {
		this.apollo = apollo.use("shared");
		this.networkInterface = this.buildNetworkInterface("baseUri");
		this.configure(this.networkInterface);
	}

	get(): ApolloBase {
		return this.apollo;
	}

	getClient(): ApolloClient {
		return this.apollo.getClient();
	}

	getMiddleware(): any {
		return applyMiddleware(this.getClient().middleware());
	}

	configure(network: HTTPNetworkInterface): void {
		this.getClient().networkInterface = network;
	}

	private buildNetworkInterface(apiBaseUri: string): HTTPNetworkInterface {
		return createNetworkInterface({
			uri: "http://localhost:62551/graphql",
			opts: {
				credentials: "same-origin",
				headers: {
					"Content-Type": "application/json"
				}
			}
		});
	}


	// aPOLLO_MODULE: any[] = [
	// 	apolloModule.forRoot(provideClient)
	// ];
}

export interface ApolloClientMap extends Dictionary<ApolloClient> {
}