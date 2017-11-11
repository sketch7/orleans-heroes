import * as _ from "lodash";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/Observable";

import { AppState } from "../../core/app.state";
import { Hero, HeroRoleType } from "./hero.model";
import { HeroService } from "./hero.service";

@Injectable()
export class HeroSelector {

	constructor(private service: HeroService) {
	}

	getById(id: string): (state: AppState) => Hero {
		return (state: AppState): Hero =>
			state.heroes[id];
	}

	getAll(roleType?: HeroRoleType): (state: AppState) => Hero[] {
		return (state: AppState): Hero[] => {
			let result: Hero[] = _.values(state.heroes);

			if (roleType) {
				result = result.filter(x => x.role === roleType);
			}

			return result;
		};
	}

	getAllGraphQL(roleType?: HeroRoleType): Observable<Hero[]> {
		// return this.service.getAllHttp(roleType);
		return this.service.getAllGraphQL(roleType);
	}
}