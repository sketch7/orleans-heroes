import "./rxjs-imports";
import { NgModule } from "@angular/core";
import { NgReduxModule, NgRedux, DevToolsExtension } from "@angular-redux/store";
import { NgReduxRouterModule, NgReduxRouter } from "@angular-redux/router";
import { provideReduxForms } from "@angular-redux/form";
import { ApolloClient } from "apollo-client";
import { createLogger } from "redux-logger";
import { applyMiddleware } from "redux";

import { AppState } from "./app.state";
import { AppEpics } from "./app.epics";
import { appReducer } from "./app.reducer";
import { client, AppApolloClient } from "./app.graphql";
import { ApolloModule } from "apollo-angular";

@NgModule({
    imports: [NgReduxModule, NgReduxRouterModule],
    providers: [AppEpics, AppApolloClient]
})
export class StoreModule {
    constructor(
        store: NgRedux<AppState>,
        devTools: DevToolsExtension,
        ngReduxRouter: NgReduxRouter,
        appEpics: AppEpics,
        apollo: AppApolloClient
    ) {
        const enhancers: any[] = [
            apollo.getMiddleware()
        ];

        if (devTools.isEnabled()) {
            enhancers.push(devTools.enhancer());
        }

        store.configureStore(
            appReducer,
            {} as any,
            [createLogger(), ...appEpics.all],
            enhancers);

        if (ngReduxRouter) {
            ngReduxRouter.initialize();
        }

        provideReduxForms(store);
    }
}