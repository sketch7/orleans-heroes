import * as _ from "lodash";
import { Dictionary } from "@ssv/core";

export function arrayToObject<T extends { id: string }>(entities: T[]): Dictionary<T> {
	return entities.reduce((obj, entity: T) =>
		({ ...obj, [entity.id]: entity }), {});
}


/**
 * Joins data between the source subprop with the target.
 *
 * @template T1 Type of the source
 * @template T2 Type of the target
 * @param {Dictionary<T1>} sourceMap Collection of data for the source.
 * @param {Dictionary<T2>} targetMap Dictionary of data for the target (to join with).
 * @param {(src: T1) => T2[]} sourceProp Select the property returning an array to iterate and merge.
 * @param {(src: T2) => any} joinBySelector Select the property from the target to use to map from. e.g. x => x.id
 * @param {keyof T1} sourcePropKey Property name to update and merge back to. e.g. "heroes"
 * @returns
 * @example
 * 	Shorthand of:
 * ```ts
 * 		if (_.isEmpty(heroState.entities)) {
			return undefined;
		}

		const categories = _.values(state.entities);
		return categories.reduce((cats, heroCategory) => {
			if (!heroCategory.heroes) {
				return cats;
			}

			const detailCategory: HeroCategory = {
				...heroCategory,
				heroes: heroCategory.heroes.map(hero => HeroState.getByKey(hero!.id)(heroState))
			};
			cats.push(detailCategory);
			return cats;
		}, [] as HeroCategory[]);
 * ```
 */
export function join<T1, T2>(
	sourceMap: Dictionary<T1>,
	targetMap: Dictionary<T2>, sourceProp: (src: T1) => T2[], joinBySelector: (src: T2) => any, sourcePropKey: keyof T1
): T1[] | undefined {
	if (_.isEmpty(targetMap)) {
		return undefined;
	}

	const sourceArray = _.values(sourceMap);
	return sourceArray.reduce((cats, heroCategory) => {
		const prop = sourceProp(heroCategory);
		if (!prop) {
			return cats;
		}

		const detailCategory: T1 = {
			...heroCategory,
			[sourcePropKey]: prop.map(hero => targetMap[joinBySelector(hero)])
		};
		cats.push(detailCategory);
		return cats;
	}, [] as T1[]);
}