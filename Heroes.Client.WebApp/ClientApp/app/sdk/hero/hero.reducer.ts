import { Action, ActionPayload } from "../../shared/redux/action.utils";
import { updateMapState, updateAllMapState } from "../../shared/redux/reducer.utils";
import { Dictionary } from "../../shared/utils";
import { HeroState, Hero } from "./hero.model";
import { HERO_ACTION_TYPE } from "./hero.action";

const INITIAL_STATE: HeroState = {};

export function heroReducer(state: HeroState = INITIAL_STATE, action: Action): HeroState {

	switch (action.type) {

		case HERO_ACTION_TYPE.getSuccess: {
			const response: ActionPayload<Hero> = action as ActionPayload<Hero>;
			return updateMapState<HeroState, Hero>(response.payload.key, state, response.payload);
		}
		case HERO_ACTION_TYPE.getAllSuccess: {
			const response: ActionPayload<Hero[]> = action as ActionPayload<Hero[]>;
			return updateAllMapState<HeroState, Hero>(state, response.payload, "key");
		}
	}

	return state;
}