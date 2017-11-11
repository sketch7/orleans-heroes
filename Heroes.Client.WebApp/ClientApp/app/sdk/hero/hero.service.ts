import * as _ from "lodash";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs/Observable";
import { Injectable } from "@angular/core";
import { Apollo } from "apollo-angular";
import gql from "graphql-tag";

import { AppApolloClient } from "../../core/app.graphql";
import { HeroRoleType, Hero } from "./hero.model";

@Injectable()
export class HeroService {

		list = [
		{
			key: "rengar",
			name: "mock data - mighty rengo",
			role: HeroRoleType.assassin,
			abilities: []
		},
		{
			key: "kha-zix",
			name: "mock data - kha zix",
			role: HeroRoleType.assassin,
			abilities: []
		},
		{
			key: "singed",
			name: "mock data - singed",
			role: HeroRoleType.tank,
			abilities: []
		}
	];

	constructor(
		private http: HttpClient,
		private apollo: Apollo,
	) {
	}

	getById(id: string): Observable<Hero> {
		return Observable.of(_.find(this.list, x => x.key === id)!);
	}

	getAll(): Observable<Hero[]> {
		return Observable.of(this.list);
	}

	getAllHttp(): Observable<Hero[]> {
		return this.http.get<Hero[]>("http://localhost:62551/api/heroes")
			.do(x => console.log("HeroService :: http response", x));
	}

	getAllGraphQL(roleType?: HeroRoleType): Observable<Hero[]> {
		console.log("HeroService :: graphQL - init");
		const query: any = gql`
        query GetAllHeroes($role: HeroRoleEnum) {
            heroes (role: $role) {
              key
              name
              role
            }
          }
        `;

		return this.apollo.query<any>({
			query
		})
			.do(x => console.log("HeroService :: graphQL - response", x))
			.map(({ data }) => data.heroes);
	}

}