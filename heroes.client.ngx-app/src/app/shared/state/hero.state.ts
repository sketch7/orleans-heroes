import * as _ from "lodash";
import { Injectable } from "@angular/core";
import { tap } from "rxjs/operators";
import { State, StateContext, Action, Selector } from "@ngxs/store";

import { Hero } from "./hero.model";
import { HeroService } from "./hero.service";

export interface Dictionary<T> {
	[key: string]: T;
}

export function arrayToObject<T extends { key: string }>(entities: T[]): Dictionary<T> {
	return entities.reduce((obj, entity: T) =>
		({ ...obj, [entity.key]: entity }), {});
}

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
}

@State<HeroStateModel>({
	name: "heroes",
	defaults: {
		entities: {}
	}
})
// eslint-disable-next-line @angular-eslint/use-injectable-provided-in
@Injectable({
	providedIn: "root"
})
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
	static getSelected(state: HeroStateModel): Hero | undefined {
		if (!state.selectedKey) {
			return undefined;
		}
		const entity = state.entities[state.selectedKey];
		return entity;
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
		ctx.patchState({
			selectedKey: key
		});
	}

	@Action(HeroActions.Add)
	add(ctx: StateContext<HeroStateModel>, { payload: hero }: HeroActions.Add) {
		const state = ctx.getState();

		ctx.patchState({
			entities: { ...state.entities, [hero.key]: hero }
		});
		// ctx.setState({
		// 	heroes: append(action.payload)
		// });
	}

}
