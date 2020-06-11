import * as _ from "lodash";
import { Injectable } from "@angular/core";
import { tap } from "rxjs/operators";
import { State, StateContext, Action, Selector, createSelector } from "@ngxs/store";
import { patch } from "@ngxs/store/operators";
import { Dictionary } from "@ssv/core";

import { arrayToObject } from "../utils";
import { Hero } from "./hero.model";
import { HeroService } from "./hero.service";

export namespace HeroActions {

	export class Add {
		static readonly type = "[Hero] Add";

		constructor(
			public payload: Hero
		) {
		}
	}

	export class Load {
		static readonly type = "[Hero] Load";
	}

	export class Select {
		static readonly type = "[Hero] Select";
		constructor(
			public key: string
		) {
		}
	}

}

export interface HeroStateModel {
	entities: Dictionary<Hero>;
	selectedKey?: string;
	recentlyViewed: string[];
}

@State<HeroStateModel>({
	name: "heroes",
	defaults: {
		entities: {},
		recentlyViewed: [],
	},
})
@Injectable()
export class HeroState {

	@Selector()
	static state(state: HeroStateModel): HeroStateModel {
		return state;
	}

	@Selector()
	static getEntities(state: HeroStateModel): Dictionary<Hero> {
		return state.entities;
	}

	@Selector()
	static getEntityList(state: HeroStateModel): Hero[] {
		return _.values(state.entities);
	}

	@Selector()
	static getRecentlyViewed(state: HeroStateModel): Hero[] {
		const recentlyViewed = state.recentlyViewed;
		return recentlyViewed.map(key => state.entities[key])
			.filter(x => !!x);
	}

	@Selector()
	static getSelected(state: HeroStateModel): Hero | undefined {
		if (!state.selectedKey) {
			return undefined;
		}
		const entity = state.entities[state.selectedKey];
		return entity;
	}

	static getPopular(count = 5) {
		return createSelector([HeroState.getEntities], (heroes: Hero[]) => {
			heroes = _.orderBy(heroes, x => x.popularity);
			heroes = _.take(heroes, count);
			return heroes;
		});
	}

	static getByKey(key: string) {
		return createSelector([HeroState], (state: HeroStateModel) => {
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

	@Action(HeroActions.Load)
	get(ctx: StateContext<HeroStateModel>) {
		return this.service.getAll().pipe(
			tap(x => ctx.patchState({ entities: arrayToObject(x || []) })),
		);
	}

	@Action(HeroActions.Select)
	select(ctx: StateContext<HeroStateModel>, { key }: HeroActions.Select) {
		let recentlyViewed = [...ctx.getState().recentlyViewed];
		recentlyViewed = [key, ...recentlyViewed];
		recentlyViewed = _.uniq(recentlyViewed);
		recentlyViewed = _.take(recentlyViewed, 3);

		ctx.setState(
			patch({
				selectedKey: key,
				recentlyViewed,
			})
		);
	}

	@Action(HeroActions.Add)
	add(ctx: StateContext<HeroStateModel>, { payload: hero }: HeroActions.Add) {
		const state = ctx.getState();

		ctx.patchState({
			entities: { ...state.entities, [hero.id]: hero }
		});
	}

}
