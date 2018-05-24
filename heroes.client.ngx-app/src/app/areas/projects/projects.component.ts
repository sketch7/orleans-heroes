import { Component } from "@angular/core";
import { Project } from "./projects.model";

@Component({
	selector: "app-projects",
	templateUrl: "./projects.component.html",
	styleUrls: ["./projects.component.scss"],
})
export class ProjectsComponent {
	projects: Project[] = [
		// js
		{
			key: "ssv-core",
			title: "@ssv/core",
			tag: "javascript",
			url: "https://github.com/sketch7/ssv-core",
			description: "typescript core functions and utils",
		},
		{
			key: "signalr-client",
			title: "@ssv/signalr-client",
			tag: "javascript",
			url: "https://github.com/sketch7/signalr-client",
			description: "SignalR client library built on top of @aspnet/signalr-client. This gives you more features and easier to use.",
		},

		// ngx
		{
			key: "ngx-command",
			title: "@ssv/ngx.command",
			tag: "angular",
			url: "https://github.com/sketch7/ngx.command",
			description:
				"Command pattern implementation for angular. Command used to encapsulate information which is needed to perform an action.",
		},
		{
			key: "angular-skeleton-app",
			title: "angular skeleton app",
			tag: "angular",
			url: "https://github.com/sketch7/angular-skeleton-app",
			description: "Angular 5.x navigation skeleton project with styling which get you started faster.",
		},
		{
			key: "ng2-heroes",
			title: "ng2 heroes app",
			tag: "angular",
			url: "https://github.com/sketch7/ng2-heroes",
			description: "angular2 heroes sample application",
		},

		// au
		{
			key: "ssv-au-core",
			title: "@ssv/au-core",
			tag: "aurelia",
			url: "https://github.com/sketch7/ssv-au-core",
			description: "Core components, utilities and services for Aurelia by Sketch7",
		},
		{
			key: "ssv-au-ui",
			title: "@ssv/au-ui",
			tag: "aurelia",
			url: "https://github.com/sketch7/ssv-au-ui",
			description: "UI components library for Aurelia by Sketch7",
		},

		// csharp
		{
			key: "fluently-http-client",
			title: "Fluently Http Client",
			tag: "csharp",
			url: "https://github.com/sketch7/FluentlyHttpClient",
			description: "Http Client for .NET Standard with fluent APIs which are intuitive, easy to use and also highly extensible.",
		},
	];
}
