# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this project.

## Regions in Repository Implementations

Repository implementations that cover multiple entity concerns must use `#region` blocks to group methods by entity, mirroring the regions in the corresponding interface. Each region is named after the entity it covers (e.g. `#region User`).

## Repository method naming convention

All repository methods must follow the `Add` / `FindBy` / `Remove` / `Update` naming pattern, consistent across `IUserRepository` and `IAuditLogRepository`.

| Operation | Method name pattern | Example |
|---|---|---|
| Create / insert | `Add` | `Add(User user)` |
| Read by primary key | `FindById` | `FindById(Guid id)` |
| Read by other criteria | `FindBy{Criteria}` | `FindByCredentials(string email, string password)` |
| Delete | `Remove` | `Remove(Guid id)` |
| Persist tracked changes | `Save` | `Save()` |

Do **not** use CRUD-prefixed names like `CreateUser`, `ReadTrackedUser`, `DeleteUser`, or `UpdateUser`. Do **not** add an `Async` suffix — all repository methods are async by convention.
