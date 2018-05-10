import { TestBed, async } from "@angular/core/testing";
import { RouterTestingModule } from "@angular/router/testing";

import { NavComponent } from "./nav.component";
import { AppSharedModule } from "../../shared";

describe("NavComponent", () => {
	beforeEach(
		async(() => {
			TestBed.configureTestingModule({
				imports: [RouterTestingModule, AppSharedModule],
				declarations: [NavComponent],
			}).compileComponents();
		}),
	);
	it(
		"should create the nav",
		async(() => {
			const fixture = TestBed.createComponent(NavComponent);
			const comp: NavComponent = fixture.debugElement.componentInstance;
			expect(comp).toBeTruthy();
		}),
	);
	it(
		`should have as appTitle set`,
		async(() => {
			const fixture = TestBed.createComponent(NavComponent);
			const comp: NavComponent = fixture.debugElement.componentInstance;
			expect(comp.appTitle).toEqual("Angular Skeleton App");
		}),
	);
	it(
		"should render appTitle in navbar",
		async(() => {
			const fixture = TestBed.createComponent(NavComponent);
			fixture.detectChanges();
			const compiled = fixture.debugElement.nativeElement;
			expect(
				compiled.querySelector(".navbar-brand").textContent,
			).toContain("Angular Skeleton App");
		}),
	);
});
