import { Routes } from "@angular/router";

import { NavComponent } from "./nav/nav.component";
import { HomeComponent } from "./home/home.component";
import { ErrorComponent } from "./error/error.component";
import { NotFoundComponent } from "./not-found/not-found.component";
import { ProjectsComponent } from "./projects/projects.component";
import { ProjectComponent } from "./projects/project.component";
import { SignalrComponent } from "./signalr/signalr.component";
import { HeroListComponent } from "./heroes/hero-list.component";
import { HeroDetailComponent } from "./heroes/hero-detail.component";
import { HeroLayoutComponent } from "./heroes/hero-layout.component";
import { HeroListContainer } from "./heroes/hero-list.container";

export const AREAS_ROUTES: Routes = [
	{ path: "", component: HomeComponent, pathMatch: "full" },
	{ path: "projects", component: ProjectsComponent },
	{
		path: "heroes", component: HeroLayoutComponent,
		children: [
			{ path: "", component: HeroListContainer, pathMatch: "full" },
			{ path: ":id", component: HeroDetailComponent }
		]
	},
	{ path: "signalr", component: SignalrComponent },
	{ path: "error", component: ErrorComponent },
	{ path: "**", component: NotFoundComponent },
];

export const AREAS_COMPONENTS = [
	NavComponent,
	ErrorComponent,
	NotFoundComponent,

	HomeComponent,
	ProjectsComponent,
	ProjectComponent,
	SignalrComponent,
	HeroListComponent,
	HeroListContainer, // todo: do we need?
	HeroLayoutComponent,
	HeroDetailComponent,
];