import { ApolloClient, createNetworkInterface } from "apollo-client";
import { ApolloModule } from "apollo-angular";

export const client: ApolloClient = new ApolloClient({
	networkInterface: createNetworkInterface({
		uri: "http://localhost:62552/graphql",
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

export function provideClient(): ApolloClient {
	return client;
}

export const APOLLO_MODULE: any[] = [
	ApolloModule.forRoot(provideClient)
];