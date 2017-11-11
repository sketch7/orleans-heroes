import { NgModule } from "@angular/core";

import { HeroService } from "./hero.service";
import { HeroSelector } from "./hero.selector";
import { HeroAction } from "./hero.action";
import { HeroEpics } from "./hero.epic";

@NgModule({
	providers: [
		HeroService,
		HeroAction,
		HeroSelector,
		HeroEpics
	]
})
export class HeroModule { }