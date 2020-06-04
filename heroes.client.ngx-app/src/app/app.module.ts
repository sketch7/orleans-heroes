import { BrowserModule } from "@angular/platform-browser";
import { NgModule } from "@angular/core";
import { ServiceWorkerModule } from "@angular/service-worker";
import { FormsModule } from "@angular/forms";
import { HttpClientModule } from "@angular/common/http";
import { ApolloModule } from "apollo-angular";
import { HttpLinkModule } from "apollo-angular-link-http";
import { NgxsModule } from "@ngxs/store";
import { NgxsLoggerPluginModule } from "@ngxs/logger-plugin";
import { NgxsReduxDevtoolsPluginModule } from "@ngxs/devtools-plugin";

import { environment } from "../environments/environment";
import { AppRoutingModule } from "./app-routing.module";
import { AppComponent } from "./app.component";
import { AREAS_COMPONENTS } from "./areas/index";
import { AppSharedModule } from "./shared";
import { HeroState } from "./shared/state/hero.state";

@NgModule({
	declarations: [AppComponent, ...AREAS_COMPONENTS],
	imports: [
		// vendors
		BrowserModule.withServerTransition({ appId: "serverApp" }),
		FormsModule,
		ApolloModule,
		HttpLinkModule,
		HttpClientModule,
		ServiceWorkerModule.register("/ngsw-worker.js", { enabled: environment.production }),
		// todo: storemodule
		NgxsModule.forRoot([HeroState], {
			developmentMode: !environment.production,
			selectorOptions: {
				injectContainerState: false,
				suppressErrors: false,
			}
		}),
		NgxsLoggerPluginModule.forRoot({}),
		NgxsReduxDevtoolsPluginModule.forRoot({

		}),

		// app
		AppSharedModule,
		AppRoutingModule,
	],
	providers: [],
	bootstrap: [AppComponent],
})
export class AppModule {}
