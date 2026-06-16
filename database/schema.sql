-- Schema PostgreSQL do Bolão Copa 2026.
-- O backend também consegue criar as tabelas automaticamente com AUTO_CREATE_DATABASE=true.

CREATE EXTENSION IF NOT EXISTS pgcrypto;

CREATE TABLE IF NOT EXISTS users (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    name varchar(120) NOT NULL UNIQUE,
    email varchar(180) UNIQUE,
    password_hash text NOT NULL,
    role varchar(30) NOT NULL DEFAULT 'participant',
    is_active boolean NOT NULL DEFAULT true,
    created_at timestamp with time zone NOT NULL DEFAULT now()
);

CREATE UNIQUE INDEX IF NOT EXISTS ux_users_name_lower ON users (lower(name));

CREATE TABLE IF NOT EXISTS teams (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    fifa_code varchar(10) NOT NULL UNIQUE,
    name varchar(120) NOT NULL,
    group_code varchar(5) NOT NULL
);

CREATE TABLE IF NOT EXISTS matches (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    match_number int NOT NULL UNIQUE,
    stage varchar(40) NOT NULL,
    group_code varchar(5),
    home_team_id uuid REFERENCES teams(id),
    away_team_id uuid REFERENCES teams(id),
    home_placeholder varchar(80),
    away_placeholder varchar(80),
    kickoff_at_utc timestamp with time zone NOT NULL,
    venue varchar(150),
    city varchar(100),
    status varchar(30) NOT NULL DEFAULT 'scheduled',
    home_score int,
    away_score int,
    winner_team_id uuid REFERENCES teams(id),
    created_at timestamp with time zone NOT NULL DEFAULT now()
);

CREATE TABLE IF NOT EXISTS predictions (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id uuid NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    match_id uuid NOT NULL REFERENCES matches(id) ON DELETE CASCADE,
    predicted_home_score int NOT NULL,
    predicted_away_score int NOT NULL,
    predicted_winner_team_id uuid REFERENCES teams(id),
    points int NOT NULL DEFAULT 0,
    is_exact_score boolean NOT NULL DEFAULT false,
    is_outcome_correct boolean NOT NULL DEFAULT false,
    is_goal_diff_correct boolean NOT NULL DEFAULT false,
    created_at timestamp with time zone NOT NULL DEFAULT now(),
    updated_at timestamp with time zone NOT NULL DEFAULT now(),
    CONSTRAINT uq_predictions_user_match UNIQUE(user_id, match_id)
);

CREATE TABLE IF NOT EXISTS scoring_rules (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    exact_score_points int NOT NULL DEFAULT 10,
    result_and_goal_diff_points int NOT NULL DEFAULT 6,
    result_only_points int NOT NULL DEFAULT 3,
    champion_points int NOT NULL DEFAULT 20,
    lock_minutes_before_match int NOT NULL DEFAULT 5
);

INSERT INTO scoring_rules (exact_score_points, result_and_goal_diff_points, result_only_points, champion_points, lock_minutes_before_match)
SELECT 10, 6, 3, 20, 5
WHERE NOT EXISTS (SELECT 1 FROM scoring_rules);
