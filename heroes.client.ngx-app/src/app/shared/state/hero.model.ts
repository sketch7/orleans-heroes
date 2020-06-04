export interface Hero {
	key: string;
	name: string;
	role: HeroRoleType;
	abilities: string[];
}

export enum HeroRoleType {
	assassin = "assassin",
	fighter = "fighter",
	mage = "mage",
	support = "support",
	tank = "tank",
	marksman = "marksman",
}