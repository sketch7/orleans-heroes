import { Subject } from "rxjs";
import { tap, takeUntil } from "rxjs/operators";
import { Component, OnDestroy } from "@angular/core";
import { Store } from "@ngxs/store";

import { HeroActions, HeroState, Hero } from "../../shared";

@Component({
	selector: "app-hero-layout-container",
	templateUrl: "./hero-layout.container.html",
	styleUrls: ["./hero-layout.container.scss"],
})
export class HeroLayoutContainer implements OnDestroy {

	popularHeroes: Hero[] | undefined;
	recentViewedHeroes: Hero[] | undefined;
	private readonly _destroy$ = new Subject<void>();

	constructor(
		store: Store,
	) {
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
	}

	ngOnDestroy(): void {
		this._destroy$.next();
		this._destroy$.complete();
	}

}
