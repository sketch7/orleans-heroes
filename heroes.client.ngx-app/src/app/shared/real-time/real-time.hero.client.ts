import { Injectable } from "@angular/core";
import { HubConnectionFactory, HubConnection } from "@ssv/signalr-client";

import { HeroHub } from "./real-time.hero.model";

const connectionKey = "hero";

@Injectable()
export class HeroRealtimeClient {

	constructor(
		private hubFactory: HubConnectionFactory
	) {
		this.hubFactory.create(
			{
				key: connectionKey,
				endpointUri: "http://localhost:6600/real-time/hero"
			}
		);
	}

	get(): HubConnection<HeroHub> {
		return this.hubFactory.get<HeroHub>(connectionKey);
	}

}