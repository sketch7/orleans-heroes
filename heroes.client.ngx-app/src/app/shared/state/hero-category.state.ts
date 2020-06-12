import * as _ from "lodash";
import { Injectable } from "@angular/core";
import { tap } from "rxjs/operators";
import { State, StateContext, Action, Selector, createSelector } from "@ngxs/store";
import { Dictionary } from "@ssv/core";

import { arrayToObject } from "../utils";
import { HeroService } from "./hero.service";
import { HeroCategory } from "./hero-category.model";
import { HeroState, HeroStateModel } from "./hero.state";

export namespace HeroCategoryActions {

	export class Load {
		static readonly type = "[Hero] Load";
	}

}

export interface HeroCategoryStateModel {
	entities: Dictionary<HeroCategory>;
}

@State<HeroCategoryStateModel>({
	name: "heroCategories",
	defaults: {
		entities: {},
	},
})
@Injectable()
export class HeroCategoryState {

	@Selector()
	static state(state: HeroCategoryStateModel): HeroCategoryStateModel {
		return state;
	}

	@Selector()
	static getEntities(state: HeroCategoryStateModel): Dictionary<HeroCategory> {
		return state.entities;
	}

	@Selector([HeroCategoryState, HeroState])
	static getEntityList(state: HeroCategoryStateModel, heroState: HeroStateModel): HeroCategory[] | undefined {
		if (_.isEmpty(heroState.entities)) {
			return undefined;
		}

		const categories = _.values(state.entities);
		return categories.reduce((cats, heroCategory) => {
			if (!heroCategory.heroes) {
				return cats;
			}

			const detailCategory: HeroCategory = {
				...heroCategory,
				heroes: heroCategory.heroes.map(hero => HeroState.getByKey(hero!.id)(heroState))
			};
			cats.push(detailCategory);
			return cats;
		}, [] as HeroCategory[]);
	}

	static getByKey(key: string) {
		return createSelector([HeroCategoryState], (state: HeroCategoryStateModel) => {
			if (!key) {
				return undefined;
			}
			const entity = state.entities[key];
			return entity;
		});
	}

	constructor(
		private service: HeroService,
	) {
	}

	@Action(HeroCategoryActions.Load)
	get(ctx: StateContext<HeroCategoryStateModel>) {
		return this.service.getAllHeroCategories().pipe(
			tap(x => ctx.patchState({ entities: arrayToObject(x || []) })),
		);
	}

}
