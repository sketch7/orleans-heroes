import { HeroRealtimeState } from "./state/hero-realtime.state";
import { HeroCategoryState } from "./state/hero-category.state";
import { HeroState } from "./state/hero.state";

export * from "./real-time/hero.hubclient";
export * from "./real-time/hero-hub.model";

export * from "./state/hero-category.model";
export * from "./state/hero-category.action";
export * from "./state/hero-category.state";

export * from "./state/hero-realtime.action";
export * from "./state/hero-realtime.state";

export * from "./state/hero.model";
export * from "./state/hero.service";
export * from "./state/hero.action";
export * from "./state/hero.state";

export * from "./app-info.service";

export * from "./shared.module";

export const HERO_STATE = [HeroState, HeroCategoryState, HeroRealtimeState];