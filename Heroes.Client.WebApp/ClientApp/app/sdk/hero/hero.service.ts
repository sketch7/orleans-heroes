import { Injectable } from "@angular/core";
import { Observable } from "rxjs/Observable";
import { HeroRoleType, Hero } from "./hero.model";

@Injectable()
export class HeroService {
    //   constructor(private apollo: Angular2Apollo) {}

    getById(id: string): Observable<Hero> {
        console.log("HeroService - getById");
        return Observable.of({
            key: "rengar",
            name: "mighty rengo",
            role: HeroRoleType.assassin,
            abilities: []
        });
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
}
