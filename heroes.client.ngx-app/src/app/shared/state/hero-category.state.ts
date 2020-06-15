import { Injectable } from "@angular/core";
import { tap } from "rxjs/operators";
import { State, StateContext, Action, Selector, createSelector } from "@ngxs/store";
import { Dictionary } from "@ssv/core";

import { arrayToObject, join } from "../utils";
import { HeroService } from "./hero.service";
import { HeroCategory } from "./hero-category.model";
import { HeroState, HeroStateModel } from "./hero.state";
import { HeroCategoryActions } from "./hero-category.action";

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
		return join(state.entities, heroState.entities, category => category.heroes, hero => hero!.id, "heroes");
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
