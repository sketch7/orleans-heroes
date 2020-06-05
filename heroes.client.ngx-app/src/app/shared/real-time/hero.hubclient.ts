import { Observable } from "rxjs";
import { Injectable } from "@angular/core";
import { HubConnectionFactory, HubConnection } from "@ssv/signalr-client";

import { environment } from "../../../environments/environment";
import { Hero } from "../state/hero.model";
import { HeroHub } from "./hero-hub.model";

const connectionKey = "hero";

@Injectable({
	providedIn: "root"
})
export class HeroHubClient {

	private connection: HubConnection<HeroHub>;

	constructor(
		private hubFactory: HubConnectionFactory
	) {
		this.hubFactory.create(
			{
				key: connectionKey,
				endpointUri: `${environment.apiRemoteBaseUrl}/real-time/hero`,
			}
		);
		this.connection = this.get();
	}

	get(): HubConnection<HeroHub> {
		return this.hubFactory.get<HeroHub>(connectionKey);
	}

	send$(): Observable<string> {
		return this.connection.on<string>("Send");
	}

	heroChanged$(): Observable<Hero> {
		return this.connection.on<Hero>("HeroChanged");
	}

	addToGroup$(groupId: string): Observable<void> {
		return this.connection.send("AddToGroup", groupId);
	}

}