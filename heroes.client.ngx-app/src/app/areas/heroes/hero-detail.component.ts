import { Component, OnDestroy } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { takeUntil, tap, map } from "rxjs/operators";
import { Subject } from "rxjs";
import { Store } from "@ngxs/store";
import { Hero, HeroState, HeroActions } from "../../shared/index";

@Component({
	selector: "app-hero-detail",
	templateUrl: "./hero-detail.component.html",
	styleUrls: ["./hero-detail.component.scss"],
})
export class HeroDetailComponent implements OnDestroy {

	private readonly _destroy$ = new Subject<void>();
	key!: string | null;
	hero: Hero | undefined;

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
