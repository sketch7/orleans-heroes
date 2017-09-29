import { combineReducers } from "redux";
import { composeReducers, defaultFormReducer } from "@angular-redux/form";
import { routerReducer } from "@angular-redux/router";

import { heroReducer } from "../sdk/hero/hero.reducer";
import { AppState } from "./app.state";

export const appReducer = composeReducers<AppState>(
    defaultFormReducer(),
    combineReducers({
        heroes: heroReducer
}));