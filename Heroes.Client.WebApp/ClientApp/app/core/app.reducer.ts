import { Reducer, combineReducers } from "redux";
import { composeReducers, defaultFormReducer } from "@angular-redux/form";
import { routerReducer } from "@angular-redux/router";

import { heroReducer } from "../sdk/hero/hero.reducer";
import { AppState } from "./app.state";

export const appReducer: Reducer<AppState> = composeReducers<AppState>(
    defaultFormReducer(),
    combineReducers({
        heroes: heroReducer
}));

// export const appReducer: Reducer<AppState> = combineReducers<AppState>(
// {
//         heroes: heroReducer
// });