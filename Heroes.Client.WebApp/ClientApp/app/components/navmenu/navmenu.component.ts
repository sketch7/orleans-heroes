import { Component, Inject } from "@angular/core";

import { CoreConfig } from "../../shared/model";
import { HeroAction } from "../../sdk/sdk-exports";
import { AppState } from "../../core/app.state";
import { NgRedux } from "@angular-redux/store";

@Component({
	selector: "nav-menu",
	templateUrl: "./navmenu.component.html",
	styleUrls: ["./navmenu.component.css"]
})
export class NavMenuComponent {

	username: string = "ho";
	constructor(
		@Inject("CORE_CONFIG") config: CoreConfig,
		private ngRedux: NgRedux<AppState>,
		private actions: HeroAction
	) {
		console.log("navmenu: ctor - conifg", config);
		this.username = config.username;

	}

	ngOnInit(): void {
		console.log("navmenu: ngOnInit - dispatch");
		this.ngRedux.dispatch(this.actions.getById("ho"));
	}
}
