import * as _ from "lodash";
import { Injectable } from "@angular/core";

import { AppState } from "../../core/app.state";
import { Hero, HeroRoleType } from "./hero.model";
import { HeroService } from "./hero.service";

@Injectable()
export class HeroSelector {

	constructor(private service: HeroService) {
	}

	getById(id: string): (state: AppState) => Hero {
		return (state: AppState): Hero => {
			return state.heroes[id];
		};
	}

	getAll(roleType: HeroRoleType | undefined = undefined): (state: AppState) => Hero[] {
		return (state: AppState): Hero[] => {
			let result: Hero[] = _.values(state.heroes);

			if (roleType) {
				result = result.filter(x => x.role === roleType);
			}

			return result;
		};
	}

	getAllGraphQL (roleType: HeroRoleType | undefined = undefined): any {
		return this.getAllGraphQL(roleType);
	}
}