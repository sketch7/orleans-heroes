import * as _ from "lodash";
import { Subject, Observable } from "rxjs";
import { tap, takeUntil } from "rxjs/operators";
import { Component, OnDestroy, ChangeDetectorRef } from "@angular/core";
import { Store } from "@ngxs/store";

import { HeroActions, HeroState, Hero, HeroCategory, HeroCategoryState, HeroCategoryActions, HeroRealtimeActions } from "../../shared";

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
		cdr: ChangeDetectorRef,
		private store: Store,
	) {
		const connect$ = store.dispatch(new HeroRealtimeActions.Connect()).pipe(
			tap(x => console.debug(">>>> connected!", x)),
			takeUntil(this._destroy$)
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
			tap(() => cdr.markForCheck()),
			takeUntil(this._destroy$)
		);

		const recentlyViewedHeroes$ = store.select(HeroState.getRecentlyViewed).pipe(
			tap(heroes => this.recentViewedHeroes = heroes),
			tap(() => cdr.markForCheck()),
			takeUntil(this._destroy$)
		);

		const heroCategories$ = store.select(HeroCategoryState.getEntityList).pipe(
			tap(heroCategories => this.heroCategories = heroCategories),
			tap(() => cdr.markForCheck()),
			takeUntil(this._destroy$)
		);

		[connect$, heroLoad$, heroCategoryLoad$, popularHeroes$, recentlyViewedHeroes$, heroCategories$]
			.forEach((x: Observable<any>) => x.subscribe());
	}

	ngOnDestroy(): void {
		this._destroy$.next();
		this._destroy$.complete();
		this.store.dispatch(new HeroRealtimeActions.Disconnect());
	}

}
