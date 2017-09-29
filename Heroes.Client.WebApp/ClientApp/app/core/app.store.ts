import { NgModule } from "@angular/core";
import { NgReduxModule, NgRedux, DevToolsExtension } from "@angular-redux/store";
import { NgReduxRouterModule, NgReduxRouter } from "@angular-redux/router";
import { provideReduxForms } from "@angular-redux/form";
import { createLogger } from "redux-logger";

import { AppState } from "./app.state";
import { appReducer } from "./app.reducer";
import { SDK_EPICS } from "../sdk/sdk-exports";

@NgModule({
    imports: [NgReduxModule, NgReduxRouterModule]
})
export class StoreModule {
    constructor(
        public store: NgRedux<AppState>,
        devTools: DevToolsExtension,
        ngReduxRouter: NgReduxRouter
    ) {
        store.configureStore(
            appReducer,
            {} as any,
            [createLogger(), ...SDK_EPICS],
            devTools.isEnabled() ? [devTools.enhancer()] : []);

        if (ngReduxRouter) {
            ngReduxRouter.initialize();
        }

        provideReduxForms(store);
    }
}