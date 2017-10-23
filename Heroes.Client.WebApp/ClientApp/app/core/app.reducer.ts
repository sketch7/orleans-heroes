import { Reducer, combineReducers } from "redux";
import { composeReducers, defaultFormReducer } from "@angular-redux/form";
import { routerReducer } from "@angular-redux/router";
import { ApolloClient } from "apollo-client";

import { heroReducer } from "../sdk/hero/hero.reducer";
import { AppState } from "./app.state";
// import { client } from "./app.graphql";

export const appReducer: Reducer<AppState> = composeReducers<AppState>(
    defaultFormReducer(),
    combineReducers({
        heroes: heroReducer,
       // apollo: client.reducer()
}));

// export const appReducer: Reducer<AppState> = combineReducers<AppState>(
// {
//         heroes: heroReducer
// });