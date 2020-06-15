import { Hero } from "./hero.model";

export namespace HeroActions {


	export class Add {
		static readonly type = "[Hero] Add";

		constructor(
			public payload: Hero
		) {
		}
	}


	export class Update {
		static readonly type = "[Hero] Update";

		constructor(
			public payload: Hero
		) {
		}
	}


	export class UpdateAll {
		static readonly type = "[Hero] UpdateAll";

		constructor(
			public payload: Hero[]
		) {
		}
	}


	export class Load {
		static readonly type = "[Hero] Load";
	}


	export class Select {
		static readonly type = "[Hero] Select";
		constructor(
			public key: string
		) {
		}
	}

}
