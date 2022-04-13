CREATE TABLE application_event(
    rowid integer PRIMARY KEY,
    id text NOT NULL,
    stream_id text NOT NULL,
    sequence_number integer NOT NULL,
    event_type text NOT NULL,
    payload text NOT NULL,
    created_at DATE DEFAULT (datetime('now'))
);

CREATE UNIQUE INDEX "uq application_event(id)" ON application_event(id);
CREATE UNIQUE INDEX "uq application_event(stream_id, sequence_number)" ON application_event(stream_id, sequence_number);

PRAGMA user_version = 1;