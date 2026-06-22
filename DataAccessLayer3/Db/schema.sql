-- School Events & Notification Center — SQLite schema
-- Run: sqlite3 school_events.db ".read schema.sql"

PRAGMA foreign_keys = ON;
PRAGMA journal_mode = WAL;

-- ── Users ────────────────────────────────────────────────────────────────────

CREATE TABLE users (
    id              TEXT PRIMARY KEY,
    email           TEXT NOT NULL UNIQUE,
    password_hash   TEXT NOT NULL,
    role            TEXT NOT NULL CHECK (role IN ('STUDENT', 'ORGANIZER')),
    display_name    TEXT NOT NULL,
    created_at      TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE INDEX idx_users_role ON users (role);

-- ── Events ───────────────────────────────────────────────────────────────────

CREATE TABLE events (
    id              TEXT PRIMARY KEY,
    organizer_id    TEXT NOT NULL,
    title           TEXT NOT NULL,
    description     TEXT NOT NULL DEFAULT '',
    starts_at       TEXT NOT NULL,
    ends_at         TEXT NOT NULL,
    capacity        INTEGER NOT NULL CHECK (capacity >= 1),
    location        TEXT,
    status          TEXT NOT NULL DEFAULT 'DRAFT'
                    CHECK (status IN ('DRAFT', 'PUBLISHED', 'CANCELLED')),
    created_at      TEXT NOT NULL DEFAULT (datetime('now')),
    updated_at      TEXT NOT NULL DEFAULT (datetime('now')),
    FOREIGN KEY (organizer_id) REFERENCES users (id) ON DELETE RESTRICT
);

CREATE INDEX idx_events_organizer_id ON events (organizer_id);
CREATE INDEX idx_events_status ON events (status);
CREATE INDEX idx_events_starts_at ON events (starts_at);

-- ── Registrations ────────────────────────────────────────────────────────────

CREATE TABLE registrations (
    id              TEXT PRIMARY KEY,
    event_id        TEXT NOT NULL,
    user_id         TEXT NOT NULL,
    status          TEXT NOT NULL CHECK (status IN ('CONFIRMED', 'WAITLISTED', 'CANCELLED')),
    registered_at   TEXT NOT NULL DEFAULT (datetime('now')),
    cancelled_at    TEXT,
    FOREIGN KEY (event_id) REFERENCES events (id) ON DELETE RESTRICT,
    FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE RESTRICT
);

CREATE UNIQUE INDEX idx_registrations_one_active_per_user_event
    ON registrations (event_id, user_id)
    WHERE status IN ('CONFIRMED', 'WAITLISTED');

CREATE INDEX idx_registrations_event_status ON registrations (event_id, status);
CREATE INDEX idx_registrations_user_id ON registrations (user_id);
CREATE INDEX idx_registrations_waitlist_fifo
    ON registrations (event_id, registered_at)
    WHERE status = 'WAITLISTED';

-- ── Notification jobs (outbox / queue) ───────────────────────────────────────

CREATE TABLE notification_jobs (
    id                  TEXT PRIMARY KEY,
    type                TEXT NOT NULL,
    payload             TEXT NOT NULL,
    status              TEXT NOT NULL DEFAULT 'PENDING'
                        CHECK (status IN ('PENDING', 'PROCESSING', 'SENT', 'FAILED')),
    attempts            INTEGER NOT NULL DEFAULT 0,
    max_attempts        INTEGER NOT NULL DEFAULT 3,
    idempotency_key     TEXT NOT NULL UNIQUE,
    last_error          TEXT,
    created_at          TEXT NOT NULL DEFAULT (datetime('now')),
    processed_at        TEXT
);

CREATE INDEX idx_notification_jobs_pending
    ON notification_jobs (created_at)
    WHERE status = 'PENDING';

CREATE INDEX idx_notification_jobs_type ON notification_jobs (type);

-- ── Notification log ───────────────────────────────────────────────────────────

CREATE TABLE notification_log (
    id                  TEXT PRIMARY KEY,
    job_id              TEXT,
    recipient_email     TEXT NOT NULL,
    type                TEXT NOT NULL,
    subject             TEXT,
    success             INTEGER NOT NULL CHECK (success IN (0, 1)),
    error_message       TEXT,
    sent_at             TEXT NOT NULL DEFAULT (datetime('now')),
    FOREIGN KEY (job_id) REFERENCES notification_jobs (id) ON DELETE SET NULL
);

CREATE INDEX idx_notification_log_job_id ON notification_log (job_id);
CREATE INDEX idx_notification_log_sent_at ON notification_log (sent_at);

CREATE UNIQUE INDEX idx_notification_log_job_success
    ON notification_log (job_id)
    WHERE success = 1 AND job_id IS NOT NULL;

-- ── Badges ───────────────────────────────────────────────────────────────────

CREATE TABLE badges (
    id              TEXT PRIMARY KEY,
    event_id        TEXT NOT NULL UNIQUE,
    name            TEXT NOT NULL,
    description     TEXT NOT NULL DEFAULT '',
    image_url       TEXT,
    created_at      TEXT NOT NULL DEFAULT (datetime('now')),
    FOREIGN KEY (event_id) REFERENCES events (id) ON DELETE CASCADE
);

-- ── Attendances (check-in) ───────────────────────────────────────────────────

CREATE TABLE attendances (
    id              TEXT PRIMARY KEY,
    registration_id TEXT NOT NULL UNIQUE,
    event_id        TEXT NOT NULL,
    user_id         TEXT NOT NULL,
    checked_in_at   TEXT NOT NULL DEFAULT (datetime('now')),
    method          TEXT NOT NULL DEFAULT 'SELF'
                    CHECK (method IN ('SELF', 'ORGANIZER', 'QR')),
    FOREIGN KEY (registration_id) REFERENCES registrations (id) ON DELETE RESTRICT,
    FOREIGN KEY (event_id) REFERENCES events (id) ON DELETE RESTRICT,
    FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE RESTRICT
);

CREATE INDEX idx_attendances_event_id ON attendances (event_id);
CREATE INDEX idx_attendances_user_id ON attendances (user_id);

-- ── User badges ──────────────────────────────────────────────────────────────

CREATE TABLE user_badges (
    id              TEXT PRIMARY KEY,
    user_id         TEXT NOT NULL,
    badge_id        TEXT NOT NULL,
    awarded_at      TEXT NOT NULL DEFAULT (datetime('now')),
    FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE,
    FOREIGN KEY (badge_id) REFERENCES badges (id) ON DELETE CASCADE,
    UNIQUE (user_id, badge_id)
);

CREATE INDEX idx_user_badges_user_id ON user_badges (user_id);
