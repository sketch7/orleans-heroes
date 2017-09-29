export interface Dictionary<T> {
	[key: string]: T;
}

/* tslint:disable:interface-over-type-literal */
/** Type which limits to not be an array */
export type NonArray = { forEach?: void };

// todo: remove once update typescript to 2.2 and use `object` instead.
/** Type which limits to non primitives - N.B. this will be removed once updating to typescript 2.2 */
// export type NonPrimitive = { charAt?: void } & { toFixed?: void } & { forEach?: void };

export type PartialObject<T> = Partial<T> & object & NonArray;