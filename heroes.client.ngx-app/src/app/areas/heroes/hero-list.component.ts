import { Component, Input } from "@angular/core";

import { Hero } from "../../shared/index";

@Component({
	selector: "app-hero-list",
	templateUrl: "./hero-list.component.html",
	styleUrls: ["./hero-list.component.scss"],
})
export class HeroListComponent {

	@Input() heroes: Hero[] | undefined;

	trackByHero(_index: number, hero: Hero): string {
		return hero.key;
	}

}
