import { Dictionary } from "@ssv/core";

export function arrayToObject<T extends { id: string }>(entities: T[]): Dictionary<T> {
	return entities.reduce((obj, entity: T) =>
		({ ...obj, [entity.id]: entity }), {});
}