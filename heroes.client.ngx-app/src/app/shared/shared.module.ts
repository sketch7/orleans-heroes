import { NgModule } from "@angular/core";
import { HubConnectionFactory } from "@ssv/signalr-client";

@NgModule({
	providers: [
		HubConnectionFactory,
	]
})
export class AppSharedModule {
}