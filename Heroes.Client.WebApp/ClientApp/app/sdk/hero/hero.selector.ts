import * as _ from "lodash";
import { Injectable } from "@angular/core";

import { Hero, HeroRoleType } from "./hero.model";
import { AppState } from "../../core/app.state";

@Injectable()
export class HeroSelector {
	getById(id: string) {
		return (state: AppState): Hero => {
			return state.heroes[id];
		};
	}

	getAll(roleType: HeroRoleType | undefined = undefined) {
		return (state: AppState): Hero[] => {
			let result: Hero[] = _.values(state.heroes);

			if (roleType) {
				result = result.filter(x => x.role === roleType);
			}

			return result;
		};
	}
}