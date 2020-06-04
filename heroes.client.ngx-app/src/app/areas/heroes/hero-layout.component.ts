import { Component } from "@angular/core";
import { Store } from "@ngxs/store";

import { HeroActions } from "../../shared";

@Component({
	selector: "app-hero-layout",
	templateUrl: "./hero-layout.component.html",
	styleUrls: ["./hero-layout.component.scss"],
})
export class HeroLayoutComponent {

	constructor(
		store: Store,
	) {
		store.dispatch(new HeroActions.Load());
	}

}
