# Security Notes: Taskify

## Phase 1 — No-login deviation (Constitution: Authentication & authorization)

Taskify phase 1 ships **without authentication** by explicit product requirement
(spec `FR-002`: "no login for this first phase"). The active identity is selected client-side
and sent to `Taskify.Api` as the `X-Taskify-User-Id` header, which the API validates against
the five seeded users. This is **not** authentication — the header is trivially spoofable.

**Phase 2 remediation plan** (required before any non-internal deployment):

1. Introduce real authentication (OIDC/OAuth2 or ASP.NET Core Identity) for all users.
2. Enforce per-resource authorization (who may move / assign / comment) — see the open
   permission-model question in the spec's Assumptions.
3. Replace the `X-Taskify-User-Id` trust header with an authenticated principal.
4. Re-run `/speckit-converge` against the amended spec.

## Data protection

- **In transit**: HTTPS (ASP.NET Core development certificate in dev; a trusted certificate in
  production).
- **At rest**: PostgreSQL data is stored unencrypted at rest in this phase. **Deferred**:
  enable at-rest encryption (encrypted volume or database TDE) in the production environment.

## Secret handling

Database connection strings and the PostgreSQL password are supplied by .NET Aspire at
runtime (auto-generated credentials) and are never committed to source control. See
`src/Taskify.AppHost/AppHost.cs`.

## Dependency hygiene

Dependency vulnerability scanning is run via `scripts/scan-dependencies.sh`
(`dotnet list package --vulnerable --include-transitive`).
