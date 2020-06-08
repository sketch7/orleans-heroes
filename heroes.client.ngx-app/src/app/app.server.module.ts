import { NgModule } from "@angular/core";
import { ServerModule, ServerTransferStateModule } from "@angular/platform-server";

import { AppModule } from "./app.module";
import { AppComponent } from "./app.component";

@NgModule({
	imports: [
		ServerTransferStateModule,
		AppModule,
		ServerModule,
	],
	bootstrap: [AppComponent],
})
export class AppServerModule { }
