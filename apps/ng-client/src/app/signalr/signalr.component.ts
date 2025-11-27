import { Component, OnInit, OnDestroy, ChangeDetectionStrategy, inject, signal, computed, DestroyRef } from "@angular/core";
import { toSignal } from "@angular/core/rxjs-interop";
import { VERSION } from "@ssv/signalr-client";
import { Subscription, map, tap } from "rxjs";

import { Hero, HeroHubClient } from "./hero.hubclient";

export interface Group {
  id: string;
  name: string;
}

@Component({
  selector: "app-signalr-sample",
  templateUrl: "./signalr.component.html",
  styleUrls: ["./signalr.component.scss"],
  changeDetection: ChangeDetectionStrategy.OnPush,
  // imports: [JsonPipe],
})
export class SignalrComponent {

  readonly signalrVersion = VERSION;

  readonly #destroyRef = inject(DestroyRef);
  readonly #hubClient = inject(HeroHubClient);

  readonly hubConnection = this.#hubClient.get();
  readonly connectionState = toSignal(this.hubConnection.connectionState$)


  readonly availableGroups: Group[] = [
    { id: "lol/hero", name: "All LoL" },
    { id: "hots/hero", name: "All HoTS" },
    { id: "lol/hero/singed", name: "Singed" },
    { id: "lol/hero/kha-zix", name: "Kha Zix" },
    { id: "hots/hero/malthael", name: "Malthael" },
    { id: "hots/hero/johanna", name: "Johanna" },
    { id: "hots/hero/keal-thas", name: "Keal-thas" },
    { id: "hots/hero/alexstrasza", name: "Alexstrasza" },
  ];
  readonly heroesState = signal<Record<string, Hero>>({});
  readonly heroes = computed(() => Object.values(this.heroesState()));
  readonly currentUser = signal("Anonymous");
  readonly isConnected = signal(false);
  readonly selectedGroupId = signal(this.availableGroups[0].id);

  private source = "SignalrComponent ::";

  private hubConnection$$ = Subscription.EMPTY;
  private onSend$$ = Subscription.EMPTY;

  private kha$$ = Subscription.EMPTY;
  private singed$$ = Subscription.EMPTY;
  private heroChange$$ = Subscription.EMPTY;
  private connectionState$$ = Subscription.EMPTY;

  constructor() {
    this.hubConnection = this.#hubClient.get();


    this.connectionState$$ = this.hubConnection.connectionState$.subscribe(x => {
      console.log(`${this.source} :: Status Changed :: ${JSON.stringify(x)}`);
    });

    this.subscribe();
    this.connect();
    // this.disconnect();

    this.#destroyRef.onDestroy(() => {
      this.connectionState$$.unsubscribe();
      this.hubConnection$$.unsubscribe();
      this.dispose();
    });

  }

  subscribe() {
    // this.singed$$ = this.hubConnection.stream<Hero>("GetUpdates", "singed")
    // 	.subscribe(x => console.log(`${this.source} stream :: singed`, x));

    this.onSend$$ = this.#hubClient.send$().subscribe((val: string) => {
      console.log(`${this.source} send :: data received >>>`, val);
    });

    this.heroChange$$ = this.#hubClient.heroChanged$().pipe(
      tap(x => console.log(`${this.source} send :: data received >>>`, x)),
      map(heroChange => {
        const currentState = this.heroesState();
        let hero = currentState[heroChange.id];
        if (hero) {
          hero = { ...hero, ...heroChange };
        } else {
          hero = heroChange;
        }
        return hero;
      }),
      tap(hero => {
        this.heroesState.update(state => ({
          ...state,
          [hero.id]: hero
        }));
      }),
    ).subscribe();
    // this.kha$$ = this.hubConnection.stream<Hero>("GetUpdates", "kha-zix")
    // 	.subscribe(x => console.log(`${this.source} stream :: kha`, x));
  }

  connectDisconnect() {
    this.connect();
    this.disconnect();
  }

  connect() {
    this.hubConnection$$ = this.hubConnection.connect(() => {
      console.log("setting data..");
      return { token: "cla-key" };
    }).subscribe(() => {
      console.log(`${this.source} connected!!`);
    });
  }

  send() {
    this.hubConnection.send("StreamUnsubscribe", "fakeMethod", "sad");
  }

  subscribeToGroup() {
    this.#hubClient.addToGroup$(this.selectedGroupId());
  }

  subscribeToGroups() {
    this.#hubClient.addToGroups$(["lol/hero", "hots/hero"]);
  }

  invoke() {
    this.hubConnection.invoke("Echo", "fucking builds")
      .subscribe(x => console.log(`${this.source} invoke :: result`, x));
  }

  setData() {
    this.hubConnection.setData(() => ({ token: "gunit-x", test: "v2" }));
    this.hubConnection.setData(() => ({ token: "cla-key", test: "hello1" }));
  }

  trackByHero(_index: number, hero: Hero): string {
    return `${hero.id}`;
  }

  disconnect() {
    this.hubConnection.disconnect().subscribe();
  }

  dispose() {
    console.log(`${this.source} disposing...`);
    this.onSend$$.unsubscribe();
    this.kha$$.unsubscribe();
    this.singed$$.unsubscribe();
    this.heroChange$$.unsubscribe();
  }

}
