import { NgModule } from "@angular/core";
import { HubConnectionFactory } from "@ssv/signalr-client";

import { HeroRealtimeClient } from "./real-time/real-time.hero.client";

import { AppInfoService } from "./app-info.service";

@NgModule({
	providers: [
		HubConnectionFactory,
		AppInfoService,
		HeroRealtimeClient
	]
})
export class AppSharedModule {
}