import { Injectable } from "@angular/core";
import { makeStateKey, TransferState } from "@angular/platform-browser";
import { Apollo } from "apollo-angular";
import { InMemoryCache } from "apollo-cache-inmemory";
import { HttpLink } from "apollo-angular-link-http";
import { ApolloBase } from "apollo-angular/Apollo";
import { ApolloClient } from "apollo-client";

@Injectable({
	providedIn: "root"
})
export class AppApolloClient {

	private apollo: ApolloBase<any>;

	constructor(
		apollo: Apollo,
		httpLink: HttpLink,
		transferState: TransferState,
	) {
		const key = "app-gql";
		const stateKey = makeStateKey<string>(`apollo-state.${key}`);
		const uri = `http://localhost:6600/graphql`; // todo: configurable
		const cache = new InMemoryCache();
		apollo.create({
			link: httpLink.create({ uri, withCredentials: true }),
			cache
		}, key);
		this.apollo = apollo.use(key);

		const isBrowser = transferState.hasKey(stateKey);
		if (isBrowser) {
			const state = transferState.get<any>(stateKey, null);
			cache.restore(state);
		} else {
			transferState.onSerialize(stateKey, () => cache.extract());
		}
	}

	get(): ApolloBase<any> {
		return this.apollo;
	}

	getClient(): ApolloClient<any> {
		return this.apollo.getClient();
	}

}
