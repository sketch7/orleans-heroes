
export type TagType = "aurelia" | "angular" | "csharp" | "javascript" | "tools";

export interface Project {
	key: string;
	title: string;
	description?: string;
	url: string;
	tag?: TagType;
}