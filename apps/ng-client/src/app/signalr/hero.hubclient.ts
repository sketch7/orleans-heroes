import { Observable } from "rxjs";
import { Injectable } from "@angular/core";
import { HubConnectionFactory, HubConnection } from "@ssv/signalr-client";

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

export interface HeroHub {
  Send: string;
  GetUpdates: string;
  HeroChanged: string;
  AddToGroup: string;
  AddToGroups: string[];
  Echo: number;
}

const connectionKey = "hero";

@Injectable({
  providedIn: "root"
})
export class HeroHubClient {

  private connection: HubConnection<HeroHub>;

  constructor(
    private hubFactory: HubConnectionFactory,
  ) {
    this.hubFactory.create(
      {
        key: connectionKey,
        endpointUri: `http://localhost:6600/real-time/hero`,
        // configureSignalRHubConnection: hub => {
        // 	hub.keepAliveIntervalInMilliseconds = 2000
        // }
      }
    );
    this.connection = this.get();
  }

  get(): HubConnection<HeroHub> {
    return this.hubFactory.get<HeroHub>(connectionKey);
  }

  send$(): Observable<string> {
    return this.connection.on<string>("Send");
  }

  heroChanged$(): Observable<Hero> {
    return this.connection.on<Hero>("HeroChanged");
  }

  addToGroup$(groupId: string): Observable<void> {
    return this.connection.send("AddToGroup", groupId);
  }

  addToGroups$(groups: string[]): Observable<void> {
    return this.connection.send("AddToGroups", groups);
  }

}
