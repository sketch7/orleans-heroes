import { tap, takeUntil } from "rxjs/operators";
import { Component, OnDestroy } from "@angular/core";
import { Store } from "@ngxs/store";

import { HeroActions } from "../../shared";
import { Subject } from "rxjs";

@Component({
	selector: "app-hero-layout",
	templateUrl: "./hero-layout.component.html",
	styleUrls: ["./hero-layout.component.scss"],
})
export class HeroLayoutComponent implements OnDestroy {

	private readonly _destroy$ = new Subject<void>();

	constructor(
		store: Store,
	) {
		store.dispatch(new HeroActions.Load()).pipe(
			tap(x => console.debug(">>>> heroes loaded!", x)),
			takeUntil(this._destroy$)
		).subscribe();
	}

	ngOnDestroy(): void {
		this._destroy$.next();
		this._destroy$.complete();
	}

}
