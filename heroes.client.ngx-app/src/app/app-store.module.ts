import { NgModule } from "@angular/core";
import { NgxsModule } from "@ngxs/store";
import { NgxsReduxDevtoolsPluginModule } from "@ngxs/devtools-plugin";
import { NgxsLoggerPluginModule } from "@ngxs/logger-plugin";
import { NgxsStoragePluginModule } from "@ngxs/storage-plugin";

import { environment } from "../environments/environment";
import { HeroState, HeroCategoryState } from "./shared";

@NgModule({
	imports: [
		NgxsModule.forRoot([HeroState, HeroCategoryState], {
			developmentMode: !environment.production,
			selectorOptions: {
				injectContainerState: false,
				suppressErrors: false,
			}
		}),
		NgxsLoggerPluginModule.forRoot({}),
		NgxsReduxDevtoolsPluginModule.forRoot({
		}),
		NgxsStoragePluginModule.forRoot({
			key: ["heroes.recentlyViewed"]
		}),
	],
})
export class AppStoreModule { }
