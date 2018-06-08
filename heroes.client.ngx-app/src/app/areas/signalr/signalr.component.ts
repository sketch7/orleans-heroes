import { Component, OnInit, OnDestroy } from "@angular/core";
import { HubConnection, ConnectionState } from "@ssv/signalr-client";
import { Subscription } from "rxjs";

import { HeroHub } from "../../shared/real-time/real-time.hero.model";
import { HeroRealtimeClient } from "../../shared/real-time/real-time.hero.client";

@Component({
	selector: "app-signalr-sample",
	templateUrl: "./signalr.component.html",
	styleUrls: ["./signalr.component.scss"],
})
export class SignalrComponent implements OnInit, OnDestroy {

	heroMessages: Hero[] = [];
	currentUser = "Anonymous";
	isConnected = false;
	connectionState: ConnectionState | undefined;

	private hubConnection!: HubConnection<HeroHub>;

	private source = "HeroListComponent ::";

	private hubConnection$$: Subscription | undefined;
	private onSend$$: Subscription | undefined;

	private kha$$: Subscription | undefined;
	private singed$$: Subscription | undefined;
	private connectionState$$: Subscription | undefined;

	constructor(
		private client: HeroRealtimeClient
	) {
		this.hubConnection = this.client.get();
	}

	ngOnInit(): void {
		this.connectionState$$ = this.hubConnection.connectionState$.subscribe(x => {
			console.log(`${this.source} :: Status Changed :: ${JSON.stringify(x)}`);
			this.connectionState = x;
		});

		this.subscribe();
		this.connect();
	}

	subscribe() {
		// this.singed$$ = this.hubConnection.stream<Hero>("GetUpdates", "singed")
		// 	.subscribe(x => console.log(`${this.source} stream :: singed`, x));

		this.onSend$$ = this.hubConnection!.on<string>("Send").subscribe((val: string) => {
			console.log(`${this.source} send :: data received >>>`, val);
		});
		// this.kha$$ = this.hubConnection.stream<Hero>("GetUpdates", "kha-zix")
		// 	.subscribe(x => console.log(`${this.source} stream :: kha`, x));
	}

	connect() {
		this.hubConnection$$ = this.hubConnection.connect(() => {
			console.log("setting data..");
			return { token: "cla-key" };
		})
			.subscribe(() => {
				console.log(`${this.source} connected!!`);
			});
	}

	send() {
		this.hubConnection.send("StreamUnsubscribe", "fakeMethod", "sad");
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
		return `${hero.id}-${hero.health}`;
	}

	disconnect() {
		this.hubConnection.disconnect().subscribe();
	}

	ngOnDestroy(): void {
		if (this.connectionState$$) {
			this.connectionState$$.unsubscribe();
		}
		if (this.hubConnection$$) {
			this.hubConnection$$.unsubscribe();
		}
		this.dispose();
	}

	dispose() {
		console.log(`${this.source} disposing...`);
		if (this.onSend$$) {
			console.log(`${this.source} disposing onSend...`);
			this.onSend$$.unsubscribe();
		}
		if (this.kha$$) {
			console.log(`${this.source} disposing kha...`);
			this.kha$$.unsubscribe();
		}
		if (this.singed$$) {
			console.log(`${this.source} disposing singed...`);
			this.singed$$.unsubscribe();
		}
	}

}

export interface Hero {
	id: string;
	name: string;
	health: number;
}

export interface HeroHub {
	Send: string;
	GetUpdates: string;
	Echo: number;
}