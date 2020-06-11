import { Hero } from "../state/hero.model";
import { HeroCategory } from "../state/hero-category.model";

export interface AppGqlQuerySchema {
	heroCategories?: HeroCategory[];
	heroes?: Hero[];
}