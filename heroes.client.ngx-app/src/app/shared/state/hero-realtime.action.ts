import { ConnectionStatus } from "@ssv/signalr-client";

export namespace HeroRealtimeActions {

	export class Connect {
		static readonly type = "[Hero Realtime] Connect";
	}

	export class Disconnect {
		static readonly type = "[Hero Realtime] Disconnect";
	}

	export class SetStatus {
		static readonly type = "[Hero Realtime] Set Status";

		constructor(
			public status: ConnectionStatus,
		) {
		}
	}
}
