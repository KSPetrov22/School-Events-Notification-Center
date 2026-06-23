# Frontend Requirements

Extracted from `task.md`.

The frontend is optional in the assignment, but if implemented it should provide
a responsive, role-aware user interface for students and organizers.

## Student UI

- Browse published events.
- View event details including:
  - title
  - time
  - place or link
  - capacity
  - whether the event is full
- Register for events.
- Cancel own registrations if allowed by the backend rule.
- View own registrations.
- View own registration statuses, including confirmed or waitlisted state.
- View own badges if the optional badge flow is implemented.

## Organizer UI

- Create draft events.
- Edit draft events.
- Publish events.
- Cancel or close events.
- Set required event fields:
  - title
  - description
  - start/end datetime, or a single start time
  - capacity, integer >= 1
  - optional location or URL text
- View confirmed registrations for own events.
- View waitlist entries for own events in FIFO order.

## Role-Based Views

- Provide distinct navigation or screens for student and organizer workflows.
- Students must not see other students' private registration data.
- Organizers must not access another organizer's event management views.
- The frontend should reflect backend permissions, while the backend remains the
  source of truth.

## Preview Mode

- Organizer can preview how a published event will appear to students before
  publishing, or use an equivalent read-only student view of a draft.

## Usability Expectations

- UI should be responsive and usable.
- Student workflows should cover browsing published events, registering,
  cancelling, and viewing own registrations.
- Organizer workflows should cover creating/editing drafts, publishing, and
  viewing registrations and waitlists.

