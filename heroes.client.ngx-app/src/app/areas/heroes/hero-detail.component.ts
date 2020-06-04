import { Component, OnDestroy } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { takeUntil, tap, map } from "rxjs/operators";
import { Subject } from "rxjs";
import { Store } from "@ngxs/store";
import { HeroState } from "app/shared";

@Component({
	selector: "app-hero-detail",
	templateUrl: "./hero-detail.component.html",
	styleUrls: ["./hero-detail.component.scss"],
})
export class HeroDetailComponent implements OnDestroy {

	private readonly _destroy$ = new Subject<void>();
	key!: string | null;

	constructor(
		activatedRoute: ActivatedRoute,
		store: Store,
	) {
		activatedRoute.paramMap.pipe(
			tap(x => console.warn(">>>> param change", x)),
			map(params => this.key = params.get("key")),
			tap(_ => store.select(HeroState.getEntityList)), // todo: get by id
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
