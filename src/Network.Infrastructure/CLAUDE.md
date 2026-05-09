# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this project.

## Regions in Repository Implementations

Repository implementations that cover multiple entity concerns must use `#region` blocks to group methods by entity, mirroring the regions in the corresponding interface. Each region is named after the entity it covers (e.g. `#region User`).

## Repository method naming convention

All repository methods must follow the `Add` / `FindBy` / `Remove` / `Update` naming pattern, consistent across `IUserRepository` and `IAuditLogRepository`.

| Operation | Method name pattern | Example |
|---|---|---|
| Create / insert | `AddAsync` | `AddAsync(User user)` |
| Read by primary key | `FindByIdAsync` | `FindByIdAsync(Guid id)` |
| Read by other criteria | `FindBy{Criteria}Async` | `FindByCredentialsAsync(string email, string password)` |
| Delete | `RemoveAsync` | `RemoveAsync(Guid id)` |
| Persist tracked changes | `UpdateAsync` | `UpdateAsync(Guid id)` |

Do **not** use CRUD-prefixed names like `CreateUser`, `ReadTrackedUser`, `DeleteUser`, or `UpdateUser`.
