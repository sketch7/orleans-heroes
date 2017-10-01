import { createEpicMiddleware, combineEpics } from "redux-observable";
import { Injectable } from "@angular/core";

import { HeroEpics } from "../sdk/hero/hero.epic";

@Injectable()
export class AppEpics {
	all: any[];
	constructor(heroEpics: HeroEpics) {
		this.all = [
			createEpicMiddleware(combineEpics(...heroEpics.epics))
		];
	}
}