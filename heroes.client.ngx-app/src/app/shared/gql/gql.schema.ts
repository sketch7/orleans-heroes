import { Hero } from "../state/hero.model";

export interface AppGqlQuerySchema {
	heroes?: Hero[]
}