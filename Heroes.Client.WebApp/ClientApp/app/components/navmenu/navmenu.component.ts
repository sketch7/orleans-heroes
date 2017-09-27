import { Component, Inject } from "@angular/core";

import { CoreConfig } from "../../shared/model";

@Component({
    selector: "nav-menu",
    templateUrl: "./navmenu.component.html",
    styleUrls: ["./navmenu.component.css"]
})
export class NavMenuComponent {

	username: string = "ho";
	constructor(@Inject("CORE_CONFIG") config: CoreConfig) {
		console.log("navmenu: ctor - conifg", config);
		this.username = config.username;
	}
}
