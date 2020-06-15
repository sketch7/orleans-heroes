export interface Hero {
	id: string;
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