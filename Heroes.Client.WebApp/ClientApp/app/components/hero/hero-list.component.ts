import { Component, OnInit } from "@angular/core";
import { NgRedux } from "@angular-redux/store";
import { Observable } from "rxjs/Observable";

import { HeroAction, HeroSelector } from "../../sdk/sdk-exports";
import { Hero } from "../../sdk/hero/hero.model";
import { AppState } from "../../core/app.state";

@Component({
	selector: "hero-list",
	templateUrl: "./hero-list.component.html"
})
export class HeroListComponent implements OnInit {

	heroes$: Observable<Hero[]>;

	constructor(
		private store: NgRedux<AppState>,
		private action: HeroAction,
		private selector: HeroSelector
	) {
	}

	ngOnInit(): void {
		// this.store.dispatch(this.action.getAll());
		// this.heroes$ = this.store.select(this.selector.getAll());
		this.heroes$ = this.selector.getAllGraphQL();
	}

	trackByHero(_index: number, hero: Hero): string {
		return hero.key;
	}

}