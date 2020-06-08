import { Dictionary } from "@ssv/core";

export function arrayToObject<T extends { key: string }>(entities: T[]): Dictionary<T> {
	return entities.reduce((obj, entity: T) =>
		({ ...obj, [entity.key]: entity }), {});
}