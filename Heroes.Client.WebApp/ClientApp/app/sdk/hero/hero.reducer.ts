import { Action, ActionPayload } from "../../shared/redux/action.utils";
import { updateMapState } from "../../shared/redux/reducer.utils";
import { Dictionary } from "../../shared/utils";
import { HeroState, Hero } from "./hero.model";
import { HERO_ACTION_TYPE } from "./hero.action";

const INITIAL_STATE: HeroState = {};

export function heroReducer(state: HeroState = INITIAL_STATE, action: Action): HeroState {

    switch (action.type) {
        case HERO_ACTION_TYPE.get:
        console.log("heroReducer :: get");
        return state;
        case HERO_ACTION_TYPE.getSuccess:
            const response: ActionPayload<Hero> = action as ActionPayload<Hero>;
            console.log("heroReducer :: getSuccess", response);
            return updateMapState<HeroState, Hero>(state, response.payload);
    }

    return state;
}