import "./rxjs-imports";
import { NgModule } from "@angular/core";
import { NgReduxModule, NgRedux, DevToolsExtension } from "@angular-redux/store";
import { NgReduxRouterModule, NgReduxRouter } from "@angular-redux/router";
import { provideReduxForms } from "@angular-redux/form";
import { createLogger } from "redux-logger";

import { AppState } from "./app.state";
import { AppEpics } from "./app.epics";
import { appReducer } from "./app.reducer";

@NgModule({
    imports: [NgReduxModule, NgReduxRouterModule],
    providers: [AppEpics]
})
export class StoreModule {
    constructor(
        store: NgRedux<AppState>,
        devTools: DevToolsExtension,
        ngReduxRouter: NgReduxRouter,
        appEpics: AppEpics
    ) {
        console.log("StoreModule: INIT", appEpics.all);
        store.configureStore(
            appReducer,
            {} as any,
            [createLogger(), ...appEpics.all],
            devTools.isEnabled() ? [devTools.enhancer()] : []);

        if (ngReduxRouter) {
            ngReduxRouter.initialize();
        }

        provideReduxForms(store);
    }
}