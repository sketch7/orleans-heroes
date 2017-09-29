import { Injectable } from "@angular/core";
import { Hero } from "./hero.model";
import { ActionPayload } from "../../shared/redux/action.utils";

const ACTION_PREFIX: string = "[Hero]";

export const HERO_ACTION_TYPE: any = {
    get: `${ACTION_PREFIX} Get`,
    getSuccess: `${ACTION_PREFIX} Get Success`,
    getFail: `${ACTION_PREFIX} Get Fail`,
};

@Injectable()
export class HeroAction {

    getById(id: string): ActionPayload<string> {
        return {
            type: HERO_ACTION_TYPE.get,
            payload: id
        };
    }

    getSuccess(hero: Hero): ActionPayload<Hero> {
        return {
            type: HERO_ACTION_TYPE.getSuccess,
            payload: hero
        };
    }

    getFail(error: any): ActionPayload<any> {
        return {
            type: HERO_ACTION_TYPE.getFail,
            payload: error
        };
    }
}
