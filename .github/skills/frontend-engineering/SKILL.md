---
name: frontend-engineering
description: "Use when implementing or refactoring frontend features, React components, pages, forms, layouts, styles, or client-side data flows in this project. Guides project inspection, architecture, TypeScript, responsive and accessible UI, RTL, validation, and verification."
---

# Frontend Engineering

Use this skill for frontend feature implementation and refactoring in this repository. Adapt to the code that exists; do not assume every frontend task belongs to the same framework or layer.

## Verified Project Context

- The frontend application is in `src/front/1/my-next-app`.
- It uses Next.js App Router, React 19, TypeScript with `strict: true`, and the `@/*` import alias.
- Styling uses Tailwind CSS 4, CSS custom properties/design tokens in `app/globals.css`, and shadcn-style components under `components/ui`.
- The root layout sets Persian (`fa`) and RTL direction and loads Vazirmatn. Preserve the established Persian/RTL experience when relevant.
- Shared UI lives under `components`; feature-specific code is grouped under `features`; route entry points live under `app`.
- Existing data patterns include feature services and types, the shared `lib/api` client and response types, Zod schemas, React hooks, and server actions. Inspect nearby code to choose the applicable pattern.
- The available npm scripts include `lint` and `build`. Check the package manifest for actual commands before choosing verification steps. There is no declared type-check script.

Treat this context as a starting point, not a substitute for inspecting the relevant files. Do not assume these conventions apply to unrelated or newly introduced parts of the repository without verification.

## Project Analysis

- Inspect the requirement and relevant existing files before proposing or implementing changes.
- Reuse verified project libraries, components, design tokens, conventions, and patterns.
- Never assume a framework, dependency, file path, or API that has not been verified.
- Ask a concise clarification question only when an essential requirement is missing and cannot be resolved safely from project context.

## Architecture

- Separate UI components, business logic, data access, and types where appropriate, following nearby project patterns.
- Prefer small, reusable, single-responsibility components. Avoid unnecessary abstractions, premature optimization, and duplicated code.
- Follow the conventions of the framework actually used in the relevant area.
- For Next.js App Router, prefer Server Components by default; use Client Components only when interactivity, state, effects, or browser APIs require them.
- Keep secrets and server-only logic out of client bundles. Use the existing server/client boundaries and data-access patterns.

## TypeScript

- Use strict, meaningful types. Avoid `any` unless there is a documented, unavoidable reason.
- Define appropriate types for component props, API responses, forms, and domain models; reuse existing types when appropriate.
- Handle `null`, `undefined`, loading, and error states explicitly. Do not hide failures behind success-shaped fallback values.

## UI and UX

- Build responsive interfaces for mobile, tablet, and desktop.
- Follow the existing design system and visual language, including established tokens, spacing, and shared UI components.
- Support accessibility through semantic HTML, labels, keyboard navigation, and visible focus states.
- Support RTL and Persian layouts where required. Check direction-sensitive alignment, spacing, icons, text, and navigation against existing project patterns.
- Include relevant loading, empty, success, validation, and error states.
- Avoid unnecessary visual complexity and inconsistent spacing.

## Data and Forms

- Follow the existing data-fetching and mutation patterns. Inspect nearby feature services, hooks, server actions, and shared API utilities before choosing an approach.
- Validate user input at the appropriate server boundary; use existing validation libraries and schemas where suitable.
- Handle API errors, pending states, and user feedback correctly.
- Do not expose credentials, access tokens, or private environment variables.
- Avoid mock data in production features unless explicitly requested.
- For Persian/Jalali date fields, reuse an installed date-picker and calendar/locale package when available. Validate dates with real Jalali calendar rules (including leap years), keep form state in a consistent Jalali representation, and convert to Gregorian `YYYY-MM-DD` only at the API boundary when the API expects Gregorian dates. Avoid timezone-sensitive conversions for date-only values.
- When using a date picker with a custom `render` input, do not assume its supplied display-value argument is a string; some picker versions can provide a non-string value. Bind the input to the component's own typed form value, or explicitly normalize/guard the library value before string operations such as `.replace()`. Verify initial render as well as date selection.

## List Page Convention

Apply this convention to **all list pages** added or refactored in this project, regardless of entity or feature. Use `features/salons/list/salons-page.tsx` as the visual and structural reference, while keeping each list's columns and data specific to its verified domain:

- Keep list UI in a focused feature component. Use the established Persian RTL breadcrumb/header with the dark fuchsia gradient, a primary create action and search field where relevant, followed by the overlapping content area.
- Use the shared table components inside the bordered, rounded panel. Match the established compact header/row sizing, spacing, and colors.
- Give every data row an operations column with a three-dot dropdown containing **مشاهده**, **ویرایش**, and **حذف** actions.
- Use the shared pagination components with the established compact rounded fuchsia styling, previous/next controls, and current/first/last page treatment.
- Preserve responsive behavior, accessible action labels (including row context for icon-only triggers), keyboard interaction, and loading, error, and empty states.
- If the domain supports an active/inactive status, show its current state and offer an operation-menu action labeled according to the next state (for example, **غیرفعال کردن** or **فعال کردن**). Verify the actual toggle endpoint and HTTP method, request/response contract, and update the relevant types/service instead of guessing. Refresh or update the list after success; surface failures without showing a success-shaped state.
- Wire delete only to a verified API operation. Because deletion is destructive, ask for explicit confirmation that identifies the selected record, prevent duplicate submissions while pending, show API errors, and refresh the list after success. Reuse the same verified mutation/confirmation flow for delete actions opened from both the list menu and a detail dialog when both surfaces expose deletion.
- Keep all actions honest: wire view/edit/status/delete only to verified routes or API operations. Do not invent endpoints or present nonfunctional controls as working; if an operation is unavailable, make that limitation clear and follow the project’s established UX.

## API-Backed List Features

When a task adds an API-backed list feature, such as a personnel list:

1. Inspect the existing feature structure (use `features/salons/list` as a local example) and the backend endpoint, request/query DTO, response DTO, and shared API response/paging types. Confirm the HTTP method, route, query parameters, response shape, and authorization requirements; never infer them from a feature name.
2. Add the feature under `src/front/1/my-next-app/features/<feature-collection>/list/`, using the existing naming convention. For a personnel list, use `features/personnel/list/`.
3. Create these four feature modules, with meaningful implementations rather than empty placeholders:
   - `use-<feature-collection>.ts` for list fetching and UI request state, following a nearby hook such as `use-salons.ts`.
   - `<feature>.types.ts` for typed request, response, and domain data, aligned to the verified API contract.
   - `<feature>.service.ts` for calls through the existing shared API client.
   - `<feature>.schema.ts` for relevant Zod validation, such as actual list filters or query parameters; do not add unrelated form schemas.
4. Put the feature UI in a focused feature component (for example, `personnels-page.tsx`) and have the hook/service provide it with typed data. Include the applicable loading, error, empty, and populated states.
5. Add the Next.js App Router entry point at `src/front/1/my-next-app/app/<route>/page.tsx` if it does not already exist. Keep it as a Server Component unless the route itself needs client behavior; otherwise render the feature component from the route. If a route already exists, inspect and integrate with it instead of overwriting it.
6. Follow the shared [List Page Convention](#list-page-convention), using the salon list as the visual reference while retaining feature-specific columns. Only change navigation or unrelated routes when the task requires it.
7. Run the relevant lint and build/type checks available in the package scripts and report exactly what was executed.

For example, implement a personnel list using the verified `/api/Personnel` endpoint and its actual paged request/response contract, not guessed field names or a mock response.

## Detail Page and Dialog Convention

Apply these rules to detail views opened from list pages or detail routes. For personnel details, use the existing dialog and the reference images in `src/front/1/my-next-app/img/personnel/` as the project's visual and content reference:

- Inspect the entity's detail DTO and API before building the view. For personnel, the verified endpoint is `GET /api/Personnel/{id}`; use the feature service and typed response, and do not invent fields or show fabricated values for data the API does not provide.
- Preserve the established detail-dialog composition: a clear title and close action, a compact entity summary with identity/status and key domain fields, icon actions where supported, a tab bar for related information, compact bordered information cards, and a footer for available actions.
- Personnel details use the five domain tabs: personal, job, financial, health, and legal/guarantor information. Keep fields and images in the appropriate tab and map verified enum values to the project's Persian labels.
- Match the reference's RTL layout, fuchsia accents, card proportions, spacing, and compact appearance. Keep the dialog responsive; at smaller viewports constrain its height and width, and make the detail content—not the header, tabs, or footer—the scrollable area.
- Use accessible dialog and tab semantics, visible focus, descriptive labels for icon-only buttons, and keyboard-operable tabs (including arrow-key movement when using a tab pattern).
- Fetch detail data for the selected ID using the existing API service. Represent loading, API error, empty/unselected, and populated states explicitly; cancel or ignore stale requests when the dialog closes or the selected ID changes. Do not use mock data in production.
- Keep actions honest. Wire print, download, edit, or delete only when their behavior is supported; otherwise omit them or show a clearly disabled control with an explanation rather than implying the action works.
- Render missing optional values with the existing empty-value convention. Preserve sensitive values according to the product's privacy expectations (for example, mask national identifiers and bank numbers); never expose credentials or tokens.
- Format API dates without accidentally converting date-only values through the browser timezone or converting a Persian-calendar year a second time. Use the verified API representation and existing date utilities.
- For reusable definition-list fields, make text direction an optional prop typed as `"ltr" | "rtl" | "auto"` and default it to `"auto"`. Override direction only for values whose format or product requirement needs it, such as phone or card numbers, and verify their displayed ordering in RTL.
- Use image cards for API-provided images with meaningful alternative text, keyboard-accessible links/actions, and a clear empty state when there are no images. Reuse the project's image and card patterns.
- Compare the finished dialog against the reference images at a matching viewport and inspect every tab, not just the first. Check that the layout does not clip important fields or create unnecessary nested scrollbars.

## Edit Forms and Dialogs

When adding edit behavior to an existing create form, preserve its established layout and validation rather than building a separate, inconsistent form:

- Inspect the current create dialog, detail DTO, update endpoint, request DTO, handler, and file-upload flow before implementation. Verify the HTTP method, route, required body fields, response shape, enum representations, and which fields are editable.
- Reuse the create dialog for editing when its fields and workflow are substantially the same. Keep create behavior unchanged, and distinguish modes with an optional entity ID and clear Persian titles/actions such as «ویرایش پرسنل» and «ذخیره تغییرات».
- Fetch the selected entity's current data when the edit dialog opens; display loading and error states, cancel or ignore stale requests, and prevent submission until the record has loaded. Map API enums and date-only values explicitly into the form's representation; Persian dates must be converted back to Gregorian only when building the API request.
- Load existing images and preserve them in the update payload unless the user removes them. Support replacing/removing images and represent explicit removal in the API contract; do not silently clear images when no new upload is selected.
- Reuse existing validation, upload restrictions, and API error handling. On successful update, close the dialog and refresh the list/detail data; on failure, retain the form and surface the error.
- Change backend request/response DTOs and handlers only when inspection shows required edit data or update behavior is missing. Keep create and existing update behavior compatible, preserve optional fields deliberately, validate related entity IDs at the server boundary, and build the affected backend projects.
- Wire edit entry points from list operations and detail actions to the same edit flow where both are available. Do not leave a working-looking edit action disabled or route it to a nonfunctional placeholder.

## Implementation Workflow

For each task:

1. Understand the requirement and inspect relevant files and existing conventions.
2. Identify the smallest appropriate implementation plan and explain which files need to change before editing.
3. Implement the feature using existing patterns and preserving unrelated user work.
4. Run the available lint, type-check, build, and relevant tests that apply to the change. Use existing project commands; do not install dependencies without clear need and user approval.
5. Fix issues caused by the implementation and report verification accurately. Never claim a command passed unless it was actually executed.
6. Summarize changed files, important decisions, assumptions or limitations, and verification results.

## Restrictions

- Do not rewrite unrelated files or implement beyond the requested scope.
- Do not install dependencies without a clear need and user approval.
- Do not overwrite existing user work.
- Do not claim tests or builds passed unless they were actually executed.
- Prefer simple, maintainable solutions over clever code.

## Response Style

- Explain technical decisions concisely.
- Show exact file paths.
- State assumptions and limitations.
- Adapt to the existing architecture instead of imposing a new one.
