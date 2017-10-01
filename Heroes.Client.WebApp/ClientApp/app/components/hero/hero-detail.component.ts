import * as _ from "lodash";
import { Observable } from "rxjs/Observable";
import { Subscription } from "rxjs/Subscription";
import { Component, OnInit, OnDestroy } from "@angular/core";
import { ActivatedRoute, ParamMap } from "@angular/router";
import { NgRedux } from "@angular-redux/store";

import { HeroAction, HeroSelector } from "../../sdk/sdk-exports";
import { Hero } from "../../sdk/hero/hero.model";
import { AppState } from "../../core/app.state";

@Component({
	selector: "hero-detail",
	templateUrl: "./hero-detail.component.html"
})
export class HeroDetailComponent implements OnInit, OnDestroy {

	hero: Hero;
	private data$$: Subscription;

	constructor(
		private store: NgRedux<AppState>,
		private action: HeroAction,
		private selector: HeroSelector,
		private route: ActivatedRoute
	) {
	}

	ngOnInit(): void {

		this.data$$ = this.route.paramMap
			.map(params => params.get("id")!)
			.do(id => this.store.dispatch(this.action.get(id)))
			.switchMap(id => this.store.select(this.selector.getById(id)))
			.do(x => console.log("hero", x))
			.do(x => this.hero = x)
			.subscribe();
	}

	ngOnDestroy(): void {
		if (this.data$$) {
			this.data$$.unsubscribe();
		}
	}

}