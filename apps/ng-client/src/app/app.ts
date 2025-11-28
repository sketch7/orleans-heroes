import { Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SignalrComponent } from "./signalr/signalr.component";

@Component({
	selector: 'app-root',
	imports: [SignalrComponent, RouterOutlet],
	templateUrl: './app.html',
	styleUrl: './app.css',
})
export class App {

	protected readonly title = signal('ng-client');
}
