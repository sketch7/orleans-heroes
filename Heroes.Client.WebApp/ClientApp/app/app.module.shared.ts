import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { HttpModule } from "@angular/http";
import { RouterModule } from "@angular/router";
import { NgReduxModule } from "@angular-redux/store";
import { NgReduxRouterModule } from "@angular-redux/router";

import { AppComponent } from "./components/app/app.component";
import { NavMenuComponent } from "./components/navmenu/navmenu.component";
import { HomeComponent } from "./components/home/home.component";
import { FetchDataComponent } from "./components/fetchdata/fetchdata.component";
import { CounterComponent } from "./components/counter/counter.component";
import { StoreModule } from "./core/app.store";
import { SDK_PROVIDERS } from "./sdk/sdk-exports";
import { HeroListComponent } from "./components/hero/hero-list.component";
import { HeroDetailComponent } from "./components/hero/hero-detail.component";
import { APOLLO_MODULE } from "./core/app.graphql";

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        CounterComponent,
        FetchDataComponent,
        HomeComponent,
        HeroListComponent,
        HeroDetailComponent
    ],
    imports: [
        CommonModule,
        HttpModule,
        FormsModule,
        RouterModule.forRoot([
            { path: "", redirectTo: "home", pathMatch: "full" },
            { path: "home", component: HomeComponent },
            { path: "counter", component: CounterComponent },
            { path: "heroes", component: HeroListComponent },
            { path: "heroes/:id", component: HeroDetailComponent },
            { path: "fetch-data", component: FetchDataComponent },
            { path: "**", redirectTo: "home" }
        ]),
        NgReduxModule,
        NgReduxRouterModule,
        StoreModule,
        APOLLO_MODULE
    ],
    providers: [
		SDK_PROVIDERS
	]
})
export class AppModuleShared {
}
