import { Injectable } from "@angular/core";
import { Epic } from "redux-observable";
import { of } from "rxjs/observable/of";

import { Action, ActionPayload } from "../../shared/redux/action.utils";
import { HeroService } from "./hero.service";
import { HeroAction, HERO_ACTION_TYPE } from "./hero.action";
import { AppState } from "../../core/app.state";

@Injectable()
export class HeroEpics {
  epics: Epic<Action, AppState>[];

    constructor(
        private service: HeroService,
        private actions: HeroAction
    ) {
        this.epics = [this.getById];
    }

    getById = (action$: any) => action$
        .ofType(HERO_ACTION_TYPE.get)
        .map((action: ActionPayload<String>) => action.payload)
        .switchMap((id: string) => this.service.getById(id)
            .map(response => this.actions.getSuccess(response))
            .catch(err => of(this.actions.getFail(err)))
        )
}
