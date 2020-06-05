import { Component, OnDestroy } from "@angular/core";
import { Store } from "@ngxs/store";

import { HeroState, Hero } from "../../shared/index";
import { Subject } from "rxjs";
import { takeUntil, tap } from "rxjs/operators";

@Component({
	selector: "app-hero-list",
	templateUrl: "./hero-list.component.html",
	styleUrls: ["./hero-list.component.scss"],
})
export class HeroListComponent implements OnDestroy {

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

	trackByHero(_index: number, hero: Hero): string {
		return hero.key;
	}

}
