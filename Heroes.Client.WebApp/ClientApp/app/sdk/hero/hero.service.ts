import { Observable } from "rxjs/Observable";
import { Injectable } from "@angular/core";

import { HeroRoleType, Hero } from "./hero.model";

@Injectable()
export class HeroService {
    //   constructor(private apollo: Angular2Apollo) {}

    getById(id: string): Observable<Hero> {
        return Observable.of({
            key: "rengar",
            name: "mighty rengo",
            role: HeroRoleType.assassin,
            abilities: []
        });
    }

    getAll(roleType: HeroRoleType | undefined): Observable<Hero[]> {
        return Observable.of([
            {
                key: "rengar",
                name: "mighty rengo",
                role: HeroRoleType.assassin,
                abilities: []
            },
            {
                key: "singed",
                name: "singed",
                role: HeroRoleType.tank,
                abilities: []
            }
        ]);
    }

    // const query = gql`
    //   query CitiesQuery {
    //     allCities {
    //       name
    //       country
    //     }
    //   }
    // `;
    // return  this.apollo.watchQuery<any>({
    //   query: query
    // }).map(({data}) => data.allCities);

}
