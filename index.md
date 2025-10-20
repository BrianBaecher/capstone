[Go To Self-Assessment](#professional-self-assessment)


[Go to Project Summary](#project-summary)

[Milestone 1 PDF](Baecher_mod1.pdf)


[Milestone 2 PDF](Baecher_mod3.pdf)


[Milestone 3 PDF](Baecher_mod5.pdf)



## "Professional Self-Assessment"
I try to write in a pragmatic, self-documenting way. My approach to creating Blazor/Angular components is focused on breaking problems into smaller testable pieces. Ideally, I want simple, composable UI components that are easy to reason about and reuse rather than monolithic entities that can only serve a single purpose. Although that is the intent, there are certain components in this project that do not achieve the standard I was hoping for. Regardless of any shortcomings in concisely written components, the inter-component communication is wired together with clear callbacks. I made liberal use of Blazor's DI capabilites to inject the various lightweight service layers into components to allow for more easily understandable and extendable connections with the backend.


I’m fairly comfortable working with both the client and the client–server integration sides of an application. I aim to design services that centralize request logic and token handling so the UI can stay focused on state and interaction. In Update procedures, I made sure to apply defensive patterns in forms (cloning edit models, validating before submit, etc.) which should reduce bugs while protecting data integrity, and should make the app easier to test, refactor, and expand.


I prioritize responsive controls, clear feedback for user actions, and sensible defaults so common tasks feel predictable. That shows up as templated data views with filtering and paging, confirmation for destructive actions (planned where missing), and immediate visual feedback on form submit or validation issues. I lean on component libraries when they speed development but am capable of customizing templates when the situation calls for it.


Security and reliability are special considerations, and to be honest I need to strengthen my understanding and abilities in this arena. In this project, I opted to persist short‑lived tokens on the client, attach auth headers centrally, and add auto‑login flows to reduce friction for returning users.


Areas of the project I’m aware are inadequate: Lack of centralized error mapping, no automated tests or even explicit cases, many service methods do not communicate HTTPResponse details back to the components that invoke them - instead returning far less descriptive types such as booleans.


I'm meant to describe how the CS program at SNHU has strengthened my understanding of the following topics. As a disclaimer, I've found the curriculum to be incredibly lackluster, and am ultimately very disappointed with it. The only silver lining I've found is that the required coursework within the math department was challenging and helped to develop some analytic muscles that have proven useful in programming.


	1. collaborating in a team environment
		It's absurd they'd even include this.
		There has not been a single collaborative project in the entirety of the curriculum. 
		As a result, the only team environments I've participated in have been outside of school.
	2. communicating with stakeholders
		I have participated in hypothetical situations where I was guided through the process of 
		parsing stakeholder requirements. Navigating the vague and poorly defined assignment 
		rubrics will have given me a bit of practice, as well.
	3. data structures and algorithms
		Courses such as discrete math and DSA have familiarized me with the basics of things like 
		graph theory, tree traversals, and some other common algorithmic knowledge.
	4. software engineering and database
		I have not learned a single thing about database operations or software engineering 
		practices that I didn't already know coming into the program, and I am far from an expert.
	5. Security
		The math courses included in the program familiarized me with cryptographic operations 
		used in schemes like SHA256, which was an interesting and educational experience.
	
---

## Project Summary
This project is a Blazor WebAssembly single‑page application that uses Radzen UI components and a REST API to manage users, destinations, reservations, and contact messages with role‑based admin tooling. It was created based off a completed project in a previous class, where we built a simple application with CRUD capabilities using the MEAN stack. For this project, I opted to revisit the project, and through using .NET and Mongo expand on its features and functionalities.\
[Goals](#goals)


[Why Blazor](#my-choice-of-blazor-webassembly)


[Architecture/Component Design](#architecture-and-component-design)


[State mngment](#state-management-and-data-flow)


[Challenges](#challenges)



---

## Goals 
- Build a responsive client UI in Blazor WebAssembly, implementing customer-facing and administrative portions of the site.
- Focus on component composition and reusability, API integration, and tokenized authentication.
- Prioritize clear data flow between services and components.

---

## My Choice of Blazor WebAssembly
- A huge advantage of .NET is the use of a single language (C#) from end‑to‑end, which streamlines development and the requisite tools used.
- Blazor, like Angular, makes interactive pages faster to implement when compared to traditional web-development.

---

## Architecture and component design
- Componentized UI: (ideally) small, focused components compose pages (for instance, `UserDataFilter`, `EditUser`, `UserSearchComponent`).
- Reuse patterns: edit form (`EditUser.razor`) is reused for user edits; parent component shows/hides the form and reacts to callbacks.
- UI toolkit: Radzen components provide templates which helps keep everything consistent, while still allowing custom templates and behavior.

Concrete examples:
- `UserSearchComponent.razor` wires `UserDataFilter` -> `SetFormUser` -> shows `EditUser`, then `OnUserEdited` calls `userDataFilter.UpdateEditedUser(...)`.
- `UserDataFilter.razor` uses a `RadzenDataGrid` with `FilterTemplate` for the `Role` column and exposes `UpdateEditedUser(User)` for parents to call.

---

## State management and data flow
- Services centralize API and state concerns:
  - `SessionState` holds currently logged-in user (used for role checks and API interactions).
- Component state is local; parent-child communication uses `EventCallback<T>` and component methods.
- Update propagation between components:
  - Edit flow returns edited user from server (`AuthService.UpdateUserAsync`) and the parent calls `UpdateEditedUser` on the grid component to replace the local item and call `grid.Reload()` so that the updated data is displayed. Blazor components tend to refresh themselves when parameters or data within them is altered, but in cases where it is not recognized, I used this pattern of communication to cause re-rendering.

Relevant lines:
- `AuthService.GetUsersAsync()` / `UpdateUserAsync()` / `DeleteUserAsync()` — `Capstone/Services/AuthService.cs`
- `UpdateEditedUser(User editedUser)` and `OnUserEditSelected` parameter — `UserDataFilter.razor`

---

## Authentication and authorization
- JWT token stored in browser local storage and attached to `HttpClient` as `Bearer` header.
- Auto‑login pattern: `AuthService.TryAutoLogin()` refreshes token + sets `SessionState`.
- UI enforcement: admin pages check `SessionState.CurrentUser.Role` (example: `TestAdmin.razor`).

Relevant file:
- `Capstone/Services/AuthService.cs` — token persistence and `TryAutoLogin()`.

---

## API integration
- Uses `HttpClient` JSON helpers: `GetFromJsonAsync`, `PostAsJsonAsync`, `PatchAsJsonAsync`, `DeleteAsync`.
- Services wrap these calls for reuse and centralized token handling.

Examples:
- `GetUsersAsync()` in `AuthService` sets `_httpClient.DefaultRequestHeaders.Authorization` then calls `GetFromJsonAsync<List<User>>($"{URI}/users")`.

---

## Challenges
- Component composition scales: designing small components (`UserDataFilter`, `EditUser`) made wiring admin workflows straightforward.
- Token persistence and auto-login: `AuthService.TryAutoLogin()` simplifies returning user sessions between page reloads.
- Defensive editing: clone bound models before editing in forms to avoid mutating parent data (`formModel = User.Clone()` in `EditUser.razor` and `formModel = SelectedDestination.Clone()` in `DestinationForm.razor`).
- Practical tradeoffs using Radzen: rapid UI development but sometimes requires custom templates (e.g., `FilterTemplate` for role filtering).

File examples:
- `Capstone/Components/AdminComponents/UserSearch/EditUser.razor` — cloning model patterns.
- `Capstone/Services/AuthService.cs` — token patterns and centralized HTTP calls.
- `Capstone/Components/AdminComponents/UserSearch/UserDataFilter.razor` — grid filtering & TODO for confirmation dialog.

---
