export interface Hero {
	key: string;
	name: string;
	role: HeroRoleType;
	abilities?: string[];
	popularity?: number;

	health?: number;
}

export enum HeroRoleType {
	assassin = "assassin",
	fighter = "fighter",
	mage = "mage",
	support = "support",
	tank = "tank",
	marksman = "marksman",
}