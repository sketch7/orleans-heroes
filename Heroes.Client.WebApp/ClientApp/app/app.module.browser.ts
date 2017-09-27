import { NgModule } from "@angular/core";
import { BrowserModule } from "@angular/platform-browser";

import { AppModuleShared } from "./app.module.shared";
import { AppComponent } from "./components/app/app.component";
import { StartupContext, CoreConfig } from "./shared/model";

declare var startupContext: StartupContext;

export function getBaseUrl(): string {
    return document.getElementsByTagName("base")[0].href;
}

export function getConfig(): CoreConfig {
    return {
        username: startupContext.username,
        isDebug: startupContext.isDebug
    };
}

@NgModule({
    bootstrap: [ AppComponent ],
    imports: [
        BrowserModule,
        AppModuleShared
    ],
    providers: [
        { provide: "BASE_URL", useFactory: getBaseUrl },
        { provide: "CORE_CONFIG", useFactory: getConfig }
    ]
})
export class AppModule {
}