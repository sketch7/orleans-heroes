import { ApolloClient } from "apollo-client";
import { ApolloModule } from "apollo-angular";

const client: ApolloClient = new ApolloClient();

export function provideClient(): ApolloClient {
  return client;
}

export const APOLLO_MODULE: any[] = [
	ApolloModule.forRoot(provideClient)
];