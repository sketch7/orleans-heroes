import * as _ from "lodash";
import { Injectable } from "@angular/core";
import { switchMap, bufferTime, filter, mergeMap, tap, distinctUntilChanged, map, takeUntil } from "rxjs/operators";
import { State, StateContext, Action, Selector, Store, Actions, ofActionSuccessful } from "@ngxs/store";
import { ConnectionStatus } from "@ssv/signalr-client";

import { HeroHubClient } from "../real-time/hero.hubclient";
import { HeroRealtimeActions } from "./hero-realtime.action";
import { HeroActions } from "./hero.action";

export interface HeroRealtimeStateModel {
	/** Determines whether the connection should be connected or not. (desired) */
	connected: boolean;

	/** Determines the status of the connection. */
	status: ConnectionStatus;
}

@State<HeroRealtimeStateModel>({
	name: "heroRealtime",
	defaults: {
		connected: false,
		status: ConnectionStatus.disconnected,
	},
})
@Injectable()
export class HeroRealtimeState {

	@Selector()
	static state(state: HeroRealtimeStateModel): HeroRealtimeStateModel {
		return state;
	}

	constructor(
		store: Store,
		actions$: Actions,
		private hubClient: HeroHubClient,
	) {
		hubClient.get().connectionState$.pipe(
			map(x => x.status),
			distinctUntilChanged(),
		).subscribe(status => store.dispatch(new HeroRealtimeActions.SetStatus(status)));

		actions$.pipe(
			ofActionSuccessful(HeroRealtimeActions.Connect),
			switchMap(() => this.hubClient.heroChanged$().pipe(
				// tap(x => console.warn(">>>> hero changed", x)),
				bufferTime(1000),
				filter(x => !_.isEmpty(x)),
				mergeMap(heroes => store.dispatch(new HeroActions.UpdateAll(heroes))),
				takeUntil(actions$.pipe(ofActionSuccessful(HeroRealtimeActions.Disconnect))),
				// mergeMap(hero => store.dispatch(new HeroActions.Update(hero))),
			)),
		).subscribe();
	}

	@Action(HeroRealtimeActions.Connect)
	connect(ctx: StateContext<HeroRealtimeStateModel>) {
		return this.hubClient.get().connect().pipe(
			switchMap(() => this.hubClient.addToGroup$("lol/hero")),
			tap(() => ctx.patchState({ connected: true })),
		);
	}

	@Action(HeroRealtimeActions.Disconnect)
	disconnect(ctx: StateContext<HeroRealtimeStateModel>) {
		return this.hubClient.get().disconnect().pipe(
			tap(() => ctx.patchState({ connected: false })),
		);
	}

	@Action(HeroRealtimeActions.SetStatus)
	setStatus(ctx: StateContext<HeroRealtimeStateModel>, { status }: HeroRealtimeActions.SetStatus) {
		ctx.patchState({ status });
	}

}
