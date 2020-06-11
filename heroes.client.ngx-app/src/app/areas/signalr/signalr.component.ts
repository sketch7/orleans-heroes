import { Component, OnInit, OnDestroy, ChangeDetectorRef } from "@angular/core";
import { Dictionary } from "@ssv/core";
import { HubConnection, ConnectionState, VERSION } from "@ssv/signalr-client";
import { Subscription } from "rxjs";

import { HeroHub, HeroHubClient, Hero } from "../../shared";

export interface Group {
	id: string;
	name: string;
}

@Component({
	selector: "app-signalr-sample",
	templateUrl: "./signalr.component.html",
	styleUrls: ["./signalr.component.scss"],
})
export class SignalrComponent implements OnInit, OnDestroy {

	heroesState: Dictionary<Hero> = {};
	heroes: Hero[] = [];
	currentUser = "Anonymous";
	isConnected = false;
	connectionState: ConnectionState | undefined;
	signalrVersion = VERSION;

	availableGroups: Group[] = [
		{ id: "hero:all", name: "All" },
		{ id: "hero:singed", name: "Singed" },
		{ id: "hero:kha-zix", name: "Kha Zix" },
		{ id: "hero:malthael", name: "Malthael" },
		{ id: "hero:johanna", name: "Johanna" },
		{ id: "hero:keal-thas", name: "Keal-thas" },
		{ id: "hero:alexstrasza", name: "Alexstrasza" },
	];
	selectedGroupId = this.availableGroups[0].id;

	private hubConnection!: HubConnection<HeroHub>;

	private source = "SignalrComponent ::";

	private hubConnection$$ = Subscription.EMPTY;
	private onSend$$ = Subscription.EMPTY;

	private kha$$ = Subscription.EMPTY;
	private singed$$ = Subscription.EMPTY;
	private heroChange$$ = Subscription.EMPTY;
	private connectionState$$ = Subscription.EMPTY;

	constructor(
		private hubClient: HeroHubClient,
		private cdr: ChangeDetectorRef,
	) {
		this.hubConnection = this.hubClient.get();
	}

	ngOnInit(): void {
		this.connectionState$$ = this.hubConnection.connectionState$.subscribe(x => {
			console.log(`${this.source} :: Status Changed :: ${JSON.stringify(x)}`);
			this.connectionState = x;
		});

		this.subscribe();
		this.connect();
	}

	ngOnDestroy(): void {
		this.connectionState$$.unsubscribe();
		this.hubConnection$$.unsubscribe();
		this.dispose();
	}

	subscribe() {
		// this.singed$$ = this.hubConnection.stream<Hero>("GetUpdates", "singed")
		// 	.subscribe(x => console.log(`${this.source} stream :: singed`, x));

		this.onSend$$ = this.hubClient.send$().subscribe((val: string) => {
			console.log(`${this.source} send :: data received >>>`, val);
		});

		this.heroChange$$ = this.hubClient.heroChanged$().subscribe(heroChange => {
			console.log(`${this.source} send :: data received >>>`, heroChange);
			let hero = this.heroesState[heroChange.id];
			if (hero) {
				hero = { ...hero, ...heroChange };
			} else {
				hero = heroChange;
			}

			this.heroesState[hero.id] = hero;
			this.heroes = Object.values(this.heroesState);
			this.cdr.markForCheck();
		});
		// this.kha$$ = this.hubConnection.stream<Hero>("GetUpdates", "kha-zix")
		// 	.subscribe(x => console.log(`${this.source} stream :: kha`, x));
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
		this.hubClient.addToGroup$(this.selectedGroupId);
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