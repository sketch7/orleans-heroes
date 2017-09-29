import { Injectable } from "@angular/core";
import { Epic } from "redux-observable";
import { of } from "rxjs/observable/of";

import { Action, ActionPayload } from "../../shared/redux/action.utils";
import { HeroService } from "./hero.service";
import { HeroAction } from "./hero.action";
import { AppState } from "../../core/app.state";

export const heroEpics: Epic<Action, AppState>[] = [];

@Injectable()
export class HeroEpics {

    constructor(
        private service: HeroService,
        private actions: HeroAction
    ) {
        heroEpics.push(this.getById);
    }

    getById = (action$: any) => action$
        .ofType(this.actions.getById)
        .map((action: ActionPayload<String>) => action.payload)
        .do((x: string) => console.log("heroEpic :: triggered", x))
        .switchMap((id: string) => this.service.getById(id)
            .do(x => console.log("heroEpic :: response", x))
            .map(response => this.actions.getSuccess(response))
            .catch(err => of(this.actions.getFail(err))));
}
