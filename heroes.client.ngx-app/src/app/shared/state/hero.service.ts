import * as _ from "lodash";
import gql from "graphql-tag";
import { Observable, of } from "rxjs";
import { tap, map } from "rxjs/operators";
import { Injectable } from "@angular/core";
import { ApolloBase } from "apollo-angular";

import { AppApolloClient } from "../gql/apollo.client";
import { AppGqlQuerySchema } from "../gql/gql.schema";
import { HeroRoleType, Hero } from "./hero.model";

@Injectable({
	providedIn: "root"
})
export class HeroService {

	private apollo: ApolloBase;
	private list: Hero[] = [
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

	constructor(
		apollo: AppApolloClient,
	) {
		this.apollo = apollo.get();
	}

	add(entity: Hero): Observable<{}> {
		this.list.push(entity);
		return of({});
	}

	getByKey(key: string): Observable<Hero | undefined> {
		const entity = _.find(this.list, x => x.key === key);
		return of(entity);
	}

	// getAll(): Observable<Hero[]> {
	// 	return of(this.list);
	// }

	// getAllHttp(): Observable<Hero[]> {
	// 	return this.http.get<Hero[]>("http://localhost:62551/api/heroes")
	// 		.do(x => console.log("HeroService :: http response", x));
	// }

	getAll(roleType?: HeroRoleType): Observable<Hero[] | undefined> {
		console.log("HeroService :: getAll");
		const query = gql`query getAllHeroes($role: HeroRole) {
	        heroes (role: $role) {
	          key
	          name
	          role
	          abilities
	        }
	      }
	    `;

		return this.apollo.query<AppGqlQuerySchema>({
			query,
			variables: {
				role: roleType,
			}
		}).pipe(
			tap(result => console.log("HeroService :: getAll result", result)),
			map(({ data }) => data.heroes),
		);
	}

}