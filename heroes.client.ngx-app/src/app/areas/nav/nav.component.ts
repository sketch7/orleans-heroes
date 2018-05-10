import {
	Component,
	OnInit,
	OnDestroy,
	Inject,
	Renderer2,
	Optional,
	ElementRef,
	ViewChild,
} from "@angular/core";
import { DOCUMENT } from "@angular/common";

import { AppInfoService } from "../../shared";

export function isHtmlLinkElement(
	element: Element,
): element is HTMLLinkElement {
	return element.tagName.toLowerCase() === "a";
}

@Component({
	selector: "app-nav",
	templateUrl: "./nav.component.html",
	styleUrls: ["./nav.component.scss"],
})
export class NavComponent implements OnInit, OnDestroy {
	links = [
		// { path: ["/"], title: "Home", activeOptions: { exact: true } },
		{ path: ["/projects"], title: "Projects" },
	];

	appTitle = this.appInfo.title;
	appVersion = this.appInfo.version;
	appEnv = this.appInfo.environment;
	isDebug = this.appInfo.isDebug;

	isMenuOpened = false;
	@ViewChild("menu") menuElementRef: ElementRef | undefined;

	private domClickListener$$!: () => void;

	constructor(
		@Optional()
		@Inject(DOCUMENT)
		private document: any,
		private appInfo: AppInfoService,
		private renderer: Renderer2,
	) {}

	ngOnInit(): void {
		this.domClickListener$$ = this.renderer.listen(
			this.document,
			"click",
			this.onBodyClick.bind(this),
		);
	}

	ngOnDestroy(): void {
		this.domClickListener$$();
	}

	onBodyClick(event: Event): void {
		if (!this.menuElementRef || !this.isMenuOpened) {
			return;
		}

		if (
			event.target === this.menuElementRef.nativeElement ||
			this.menuElementRef.nativeElement.contains(event.target as Node)
		) {
			const target = event.target as Element;
			if (!isHtmlLinkElement(target)) {
				return;
			}
		}
		this.isMenuOpened = false;
	}
}
