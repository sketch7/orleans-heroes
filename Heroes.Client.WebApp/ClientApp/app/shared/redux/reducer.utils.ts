import { PartialObject, Dictionary } from "../utils";

// todo: use object spread instead of `Object.assign` when fixed with generics https://github.com/Microsoft/TypeScript/issues/10727

/**
 * Updates the state by merging the changes and return a new state value.
 *
 * @template TState
 * @param {TState} state original state reference.
 * @param {PartialObject<TState>} changes changes to be merged.
 */
export function updateState<TState>(state: TState, changes: PartialObject<TState>): TState {
	return Object.assign({}, state, changes) as TState;
}

/**
 * Updates a state property by merging the changes into the state property and it also returns a new state.
 */
export function updateStateItem<TState, TStateItem>(
	params: { state: TState, propertyName: keyof TState, item: TStateItem, changes: PartialObject<TStateItem> }
): TState {
	const newSubState: TStateItem = Object.assign({}, params.item, params.changes);
	return Object.assign({}, params.state, { [params.propertyName]: newSubState });
}

/**
 * Updates (add/update) an item within the state (which is a dictionary/map state)
 * with the `key` provided, by finding, merging the changes and returns a new state.
 *
 * @param {string} key key to find and update.
 * @param {TState} state original state reference.
 * @param {Partial<TStateItem>} changes item changes to be added/updated.
 * @returns {TState} returns a new state with changes applied.
 */
export function updateMapState<TState extends Dictionary<TStateItem>, TStateItem>(
	state: TState, changes: PartialObject<TStateItem>, keySelector: keyof TStateItem | "id" = "id"): TState {
	return Object.assign({}, state, {
		[keySelector]: Object.assign({}, state[keySelector], changes)
	});
}

/**
 * Updates (add/update) a collection of items within the state (which is a dictionary/map state) using the
 * property `keySelector` (default `id`) as the id within the provided item.
 * Finding, merging each individual item changes and returns a new state.
 *
 * @param {TState} state original state reference.
 * @param {PartialObject<TStateItem>[]} changes list of changes to be added/updated.
 * @param {(keyof TStateItem | "id")} [keySelector="id"] Property name used as the key within the model.
 * @returns {TState} returns a new state with the changes applied.
 */
export function updateAllMapState<TState extends Dictionary<TStateItem>, TStateItem>
	(state: TState, changes: PartialObject<TStateItem>[], keySelector: keyof TStateItem | "id" = "id"): TState {
	const newState: TState = Object.assign({}, state);

	for (const item of changes as (PartialObject<TStateItem> & Dictionary<string>)[]) {
		if (!item) {
			continue;
		}
		const stateItem: TStateItem = Object.assign({}, newState[item[keySelector]], item);
		newState[item[keySelector]] = stateItem;
	}
	return newState;
}