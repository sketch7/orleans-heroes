// note: keep pascal case since this need to match with signalr server method names.
export interface HeroHub {
	Send: string;
	GetUpdates: string;
	Echo: number;
}