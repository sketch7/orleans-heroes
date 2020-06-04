import { NgModule } from "@angular/core";
import { NgxsModule } from "@ngxs/store";
import { NgxsReduxDevtoolsPluginModule } from "@ngxs/devtools-plugin";
import { NgxsLoggerPluginModule } from "@ngxs/logger-plugin";

import { environment } from "../environments/environment";
import { HeroState } from "./shared";

@NgModule({
	imports: [
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
	],
})
export class AppStoreModule { }
