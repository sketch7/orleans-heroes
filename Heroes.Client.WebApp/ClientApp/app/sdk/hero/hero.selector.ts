import { Injectable } from "@angular/core";

import { Hero } from "./hero.model";
import { AppState } from "../../core/app.state";

@Injectable()
export class HeroSelector {
	getById(id: string) {
		return (state: AppState): Hero  => {
			return state.heroes[id];
		};
	}
}