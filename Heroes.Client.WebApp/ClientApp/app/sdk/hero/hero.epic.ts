import { Injectable } from "@angular/core";
import { Epic } from "redux-observable";
import { of } from "rxjs/observable/of";

import { Action, ActionPayload } from "../../shared/redux/action.utils";
import { HeroAction, HERO_ACTION_TYPE } from "./hero.action";
import { AppState } from "../../core/app.state";
import { HeroService } from "./hero.service";
import { HeroRoleType } from "./hero.model";

@Injectable()
export class HeroEpics {
    epics: Epic<Action, AppState>[];

    constructor(
        private service: HeroService,
        private actions: HeroAction
    ) {
        this.epics = [this.getById, this.getAll];
    }

    getById = (action$: any) => action$
        .ofType(HERO_ACTION_TYPE.get)
        .map((action: ActionPayload<String>) => action.payload)
        .switchMap((id: string) => this.service.getById(id)
            .map(response => this.actions.getSuccess(response))
            .catch(err => of(this.actions.getFail(err)))
        )

    getAll = (action$: any) => action$
        .ofType(HERO_ACTION_TYPE.getAll)
        .map((action: ActionPayload<HeroRoleType | undefined>) => action.payload)
        .switchMap((roleType: HeroRoleType | undefined) => this.service.getAll(roleType)
            .map(response => this.actions.getAllSuccess(response))
            .catch(err => of(this.actions.getAllFail(err)))
        )
}
