import { Subject } from "rxjs";
import { tap, takeUntil } from "rxjs/operators";
import { Component, OnDestroy } from "@angular/core";
import { Store } from "@ngxs/store";

import { HeroActions, HeroState, Hero } from "../../shared";

@Component({
	selector: "app-hero-layout",
	templateUrl: "./hero-layout.component.html",
	styleUrls: ["./hero-layout.component.scss"],
})
export class HeroLayoutComponent implements OnDestroy {

	popularHeroes: Hero[] | undefined;
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
	}

	ngOnDestroy(): void {
		this._destroy$.next();
		this._destroy$.complete();
	}

}
