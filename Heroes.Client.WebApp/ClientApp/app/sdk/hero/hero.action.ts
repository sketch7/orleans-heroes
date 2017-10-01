import { Injectable } from "@angular/core";

import { ActionPayload } from "../../shared/redux/action.utils";
import { Hero, HeroRoleType } from "./hero.model";

const ACTION_PREFIX: string = "[Hero]";

export const HERO_ACTION_TYPE: any = {
    get: `${ACTION_PREFIX} Get`,
    getSuccess: `${ACTION_PREFIX} Get Success`,
    getFail: `${ACTION_PREFIX} Get Fail`,

    getAll: `${ACTION_PREFIX} Get All`,
    getAllSuccess: `${ACTION_PREFIX} Get All Success`,
    getAllFail: `${ACTION_PREFIX} Get All Fail`,

};

@Injectable()
export class HeroAction {

    get(id: string): ActionPayload<string> {
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

    getAll(roleType: HeroRoleType | undefined = undefined): ActionPayload<HeroRoleType | undefined> {
        return {
            type: HERO_ACTION_TYPE.getAll,
            payload: roleType
        };
    }

    getAllSuccess(heroes: Hero[]): ActionPayload<Hero[]> {
        return {
            type: HERO_ACTION_TYPE.getAllSuccess,
            payload: heroes
        };
    }

    getAllFail(error: any): ActionPayload<any> {
        return {
            type: HERO_ACTION_TYPE.getAllFail,
            payload: error
        };
    }
}
