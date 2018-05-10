import { Component } from "@angular/core";

import { AppInfoService } from "../../shared";

@Component({
	selector: "app-home",
	templateUrl: "./home.component.html",
	styleUrls: ["./home.component.scss"],
})
export class HomeComponent {
	title = this.appInfo.title;

	constructor(private appInfo: AppInfoService) {}
}
