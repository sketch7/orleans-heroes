import { Subject } from "rxjs";
import { takeUntil, tap, map } from "rxjs/operators";
import { Component, OnDestroy } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { Store } from "@ngxs/store";

import { Hero, HeroState, HeroActions } from "../../shared/index";

@Component({
	selector: "app-hero-detail-container",
	templateUrl: "./hero-detail.container.html",
	styleUrls: ["./hero-detail.container.scss"],
})
export class HeroDetailContainer implements OnDestroy {

	key!: string | null;
	hero: Hero | undefined;
	private readonly _destroy$ = new Subject<void>();

	constructor(
		activatedRoute: ActivatedRoute,
		store: Store,
	) {
		store.select(HeroState.getSelected).pipe(
			tap(x => this.hero = x),
			takeUntil(this._destroy$),
		).subscribe();

		activatedRoute.paramMap.pipe(
			map(params => this.key = params.get("id") as string),
			tap(key => store.dispatch(new HeroActions.Select(key))),
			// switchMap(() => store.select(HeroState.getByKey("rengar"))), // todo: get by id
			// tap(x => console.warn(">>>> getByKey", x)),
			takeUntil(this._destroy$),
		).subscribe();
	}

	// ngOnInit(): void {
	// 	throw new Error("Method not implemented.");
	// }

	ngOnDestroy(): void {
		this._destroy$.next();
		this._destroy$.complete();
	}

}
