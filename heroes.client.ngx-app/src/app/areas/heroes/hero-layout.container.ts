import * as _ from "lodash";
import { Subject, Observable } from "rxjs";
import { tap, takeUntil, mergeMap, switchMap, bufferTime, filter } from "rxjs/operators";
import { Component, OnDestroy, ChangeDetectorRef } from "@angular/core";
import { Store } from "@ngxs/store";

import { HeroActions, HeroState, Hero, HeroCategory, HeroHubClient, HeroCategoryState, HeroCategoryActions } from "../../shared";

@Component({
	selector: "app-hero-layout-container",
	templateUrl: "./hero-layout.container.html",
	styleUrls: ["./hero-layout.container.scss"],
})
export class HeroLayoutContainer implements OnDestroy {

	popularHeroes: Hero[] | undefined;
	recentViewedHeroes: Hero[] | undefined;
	heroCategories: HeroCategory[] | undefined;
	private readonly _destroy$ = new Subject<void>();

	constructor(
		store: Store,
		hubClient: HeroHubClient,
		cdr: ChangeDetectorRef,
	) {
		const hubConnect$ = hubClient.get().connect().pipe(
			takeUntil(this._destroy$),
			switchMap(() => hubClient.addToGroup$("lol/hero")),
		);

		// todo: move to state?
		const heroChanged$ = hubClient.heroChanged$().pipe(
			// tap(x => console.warn(">>>> hero changed", x)),
			bufferTime(500),
			filter(x => !_.isEmpty(x)),
			mergeMap(heroes => store.dispatch(new HeroActions.UpdateAll(heroes))),
			// mergeMap(hero => store.dispatch(new HeroActions.Update(hero))),
			tap(() => cdr.markForCheck()),
		);

		const heroLoad$ = store.dispatch(new HeroActions.Load()).pipe(
			tap(x => console.debug(">>>> heroes loaded!", x)),
			takeUntil(this._destroy$)
		);

		const heroCategoryLoad$ = store.dispatch(new HeroCategoryActions.Load()).pipe(
			tap(x => console.debug(">>>> hero cateogries loaded!", x)),
			takeUntil(this._destroy$)
		);

		const popularHeroes$ = store.select(HeroState.getPopular(3)).pipe(
			tap(heroes => this.popularHeroes = heroes),
			takeUntil(this._destroy$)
		);

		const recentlyViewedHeroes$ = store.select(HeroState.getRecentlyViewed).pipe(
			tap(heroes => this.recentViewedHeroes = heroes),
			takeUntil(this._destroy$)
		);

		const heroCategories$ = store.select(HeroCategoryState.getEntityList).pipe(
			tap(heroCategories => this.heroCategories = heroCategories),
			takeUntil(this._destroy$)
		);

		[hubConnect$, heroChanged$, heroLoad$, heroCategoryLoad$, popularHeroes$, recentlyViewedHeroes$, heroCategories$]
			.forEach((x: Observable<any>) => x.subscribe());
	}

	ngOnDestroy(): void {
		this._destroy$.next();
		this._destroy$.complete();
	}

}
