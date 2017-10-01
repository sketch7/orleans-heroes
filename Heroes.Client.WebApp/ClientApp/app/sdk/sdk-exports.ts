import { Provider } from "@angular/core";
import { createEpicMiddleware, combineEpics } from "redux-observable";

import { HeroService } from "./hero/hero.service";
import { HeroSelector } from "./hero/hero.selector";
import { HeroAction } from "./hero/hero.action";
import { HeroEpics } from "./hero/hero.epic";

export { HeroService } from "./hero/hero.service";
export { HeroSelector } from "./hero/hero.selector";
export { HeroAction } from "./hero/hero.action";

export const SDK_PROVIDERS: Provider[] = [
	HeroService,
	HeroAction,
	HeroSelector,
	HeroEpics
];