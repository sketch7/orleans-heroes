import { Hero } from "./hero.model";

export interface HeroCategory {
	id: string;
	title: string;
	heroes: Partial<Hero[]>;
}