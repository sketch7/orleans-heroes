import * as _ from "lodash";
import { Subject } from "rxjs";
import { tap, takeUntil, mergeMap, switchMap, bufferTime, filter } from "rxjs/operators";
import { Component, OnDestroy, ChangeDetectorRef } from "@angular/core";
import { Store } from "@ngxs/store";

import { HeroActions, HeroState, Hero, HeroCategory, HeroHubClient } from "../../shared";
import { HeroCategoryState } from "app/shared/state/hero-category.state";

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
		hubClient.get().connect().pipe(
			takeUntil(this._destroy$),
			switchMap(() => hubClient.addToGroup$("lol/hero")),
		).subscribe();

		hubClient.heroChanged$().pipe(
			// tap(x => console.warn(">>>> hero changed", x)),
			bufferTime(500),
			filter(x => !_.isEmpty(x)),
			mergeMap(heroes => store.dispatch(new HeroActions.UpdateAll(heroes))),
			// mergeMap(hero => store.dispatch(new HeroActions.Update(hero))),
			tap(() => cdr.markForCheck()),
		).subscribe();

		store.dispatch(new HeroActions.Load()).pipe(
			tap(x => console.debug(">>>> heroes loaded!", x)),
			takeUntil(this._destroy$)
		).subscribe();

		store.select(HeroState.getPopular(3)).pipe(
			tap(heroes => this.popularHeroes = heroes),
			takeUntil(this._destroy$)
		).subscribe();

		store.select(HeroState.getRecentlyViewed).pipe(
			tap(heroes => this.recentViewedHeroes = heroes),
			takeUntil(this._destroy$)
		).subscribe();

		store.select(HeroCategoryState.getEntityList).pipe(
			tap(heroCategories => this.heroCategories = heroCategories),
			takeUntil(this._destroy$)
		).subscribe();
	}

	ngOnDestroy(): void {
		this._destroy$.next();
		this._destroy$.complete();
	}

}
