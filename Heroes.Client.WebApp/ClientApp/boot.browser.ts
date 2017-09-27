import "reflect-metadata";
import "zone.js";
import "bootstrap";
import { enableProdMode, NgModuleRef } from "@angular/core";
import { platformBrowserDynamic } from "@angular/platform-browser-dynamic";

import { AppModule } from "./app/app.module.browser";

if (module.hot) {
    module.hot.accept();
    module.hot.dispose(() => {
        // before restarting the app, we create a new root element and dispose the old one
        const oldRootElem: Element | null = document.querySelector("app");
        const newRootElem: HTMLElement = document.createElement("app");
        oldRootElem!.parentNode!.insertBefore(newRootElem, oldRootElem);
        modulePromise.then(appModule => appModule.destroy());
    });
} else {
    enableProdMode();
}

// note: @ng-tools/webpack looks for the following expression when performing production
// builds. Don"t change how this line looks, otherwise you may break tree-shaking.
const modulePromise: Promise<NgModuleRef<AppModule>> = platformBrowserDynamic().bootstrapModule(AppModule);
