import { Dictionary } from "../../shared/utils";

export interface HeroState extends Dictionary<Hero> {
}

export interface Hero {
	key: string;
	name: string;
	role: HeroRoleType;
	abilities: string[];
}

export enum HeroRoleType {
	assassin = 1,
	fighter = 2,
	mage = 3,
	support = 4,
	tank = 5,
	marksman = 6,
}