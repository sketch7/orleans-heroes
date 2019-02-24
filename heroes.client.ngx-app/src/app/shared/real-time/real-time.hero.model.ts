// note: keep pascal case since this need to match with signalr server method names.
export interface HeroHub {
	Send: string;
	GetUpdates: string;
	HeroChanged: string;
	AddToGroup: string;
	Echo: number;
}

export interface UserNotificationHub {
	Broadcast: string;
	MessageCount: string;
}