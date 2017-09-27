import "reflect-metadata";
import "zone.js";
import "rxjs/add/operator/first";
import { APP_BASE_HREF } from "@angular/common";
import { enableProdMode, ApplicationRef, NgZone, ValueProvider, Injectable, Provider } from "@angular/core";
import { platformDynamicServer, PlatformState, INITIAL_CONFIG } from "@angular/platform-server";
import { createServerRenderer, RenderResult } from "aspnet-prerendering";

import { AppModule } from "./app/app.module.server";
import { StartupContext, CoreConfig } from "./app/shared/model";

enableProdMode();

export default createServerRenderer(params => {
    const startupContext: StartupContext = params.data;

    const coreConfig: CoreConfig = {
        username: startupContext.username,
        isDebug: startupContext.isDebug
    };

    const providers: Provider[] = [
        { provide: INITIAL_CONFIG, useValue: { document: "<app></app>", url: params.url } },
        { provide: APP_BASE_HREF, useValue: params.baseUrl },
        { provide: "BASE_URL", useValue: params.origin + params.baseUrl },
        { provide: "CORE_CONFIG", useValue: coreConfig },
    ];

    return platformDynamicServer(providers).bootstrapModule(AppModule).then(moduleRef => {
        const appRef: ApplicationRef = moduleRef.injector.get(ApplicationRef);
        const state: PlatformState = moduleRef.injector.get(PlatformState);
        const zone: NgZone = moduleRef.injector.get(NgZone);

        return new Promise<RenderResult>((resolve, reject) => {
            zone.onError.subscribe((errorInfo: any) => reject(errorInfo));
            appRef.isStable.first(isStable => isStable).subscribe(() => {
                // because 'onStable' fires before 'onError', we have to delay slightly before
                // completing the request in case there's an error to report
                setImmediate(() => {
                    resolve({
                        html: state.renderToString(),
                        globals: {
                            startupContext: startupContext
                        }
                    });
                    moduleRef.destroy();
                });
            });
        });
    });
});
