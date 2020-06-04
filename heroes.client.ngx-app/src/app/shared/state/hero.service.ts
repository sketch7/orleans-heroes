import * as _ from "lodash";
import { Observable, of } from "rxjs";
import { Injectable } from "@angular/core";
// import gql from "graphql-tag";

import { HeroRoleType, Hero } from "./hero.model";

@Injectable({
	providedIn: "root"
})
export class HeroService {

	list: Hero[] = [
		{
			key: "rengar",
			name: "Rengar (mock)",
			role: HeroRoleType.assassin,
			abilities: []
		},
		{
			key: "kha-zix",
			name: "Kha'Zix (mock)",
			role: HeroRoleType.assassin,
			abilities: []
		},
		{
			key: "singed",
			name: "Singed (mock)",
			role: HeroRoleType.tank,
			abilities: []
		}
	];

	add(entity: Hero): Observable<{}> {
		this.list.push(entity);
		return of({});
	}

	getByKey(key: string): Observable<Hero | undefined> {
		const entity = _.find(this.list, x => x.key === key);
		return of(entity);
	}

	getAll(): Observable<Hero[]> {
		return of(this.list);
	}

	// getAllHttp(): Observable<Hero[]> {
	// 	return this.http.get<Hero[]>("http://localhost:62551/api/heroes")
	// 		.do(x => console.log("HeroService :: http response", x));
	// }

	// getAllGraphQL(roleType?: HeroRoleType): Observable<Hero[]> {
	// 	console.log("HeroService :: graphQL - init");
	// 	const query = gql`
	//     query GetAllHeroes($role: HeroRoleEnum) {
	//         heroes (role: $role) {
	//           key
	//           name
	//           role
	//         }
	//       }
	//     `;

	// 	return this.apollo.query<any>({
	// 		query
	// 	})
	// 		.do(x => console.log("HeroService :: graphQL - response", x))
	// 		.map(({ data }) => data.heroes);
	// }

}