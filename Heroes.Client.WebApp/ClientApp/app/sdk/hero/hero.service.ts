import * as _ from "lodash";
import { Observable } from "rxjs/Observable";
import { Injectable } from "@angular/core";

import { HeroRoleType, Hero } from "./hero.model";

@Injectable()
export class HeroService {
    //   constructor(private apollo: Angular2Apollo) {}

    list = [
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
    ];

    getById(id: string): Observable<Hero> {
        return Observable.of(_.find(this.list, x => x.key === id)!).delay(3000);
    }

    getAll(roleType: HeroRoleType | undefined): Observable<Hero[]> {
        return Observable.of(this.list);
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
