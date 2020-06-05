import { Subject } from "rxjs";
import { takeUntil, tap } from "rxjs/operators";
import { Component, OnDestroy } from "@angular/core";
import { Store } from "@ngxs/store";

import { HeroState, Hero } from "../../shared";

@Component({
	selector: "app-hero-list-page",
	templateUrl: "./hero-list.container.html",
	styleUrls: ["./hero-list.container.scss"],
})
export class HeroListContainer implements OnDestroy {

	heroes!: Hero[];
	private readonly _destroy$ = new Subject<void>();

	constructor(
		store: Store,
	) {
		store.select(HeroState.getEntityList).pipe(
			tap(heroes => this.heroes = heroes),
			takeUntil(this._destroy$)
		).subscribe();
	}

	ngOnDestroy(): void {
		this._destroy$.next();
		this._destroy$.complete();
	}

}
