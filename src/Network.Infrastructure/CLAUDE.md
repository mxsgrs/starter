# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this project.

## Repository method naming convention

All repository methods must follow the `Add` / `FindBy` / `Remove` / `Update` naming pattern, consistent across `IUserRepository`, `IAuditLogRepository`, and `ISecurityNoteRepository`.

| Operation | Method name pattern | Example |
|---|---|---|
| Create / insert | `AddAsync` | `AddAsync(User user)` |
| Read by primary key | `FindByIdAsync` | `FindByIdAsync(Guid id)` |
| Read by other criteria | `FindBy{Criteria}Async` | `FindByCredentialsAsync(string email, string password)` |
| Delete | `RemoveAsync` | `RemoveAsync(Guid id)` |
| Persist tracked changes | `UpdateAsync` | `UpdateAsync(Guid id)` |

Do **not** use CRUD-prefixed names like `CreateUser`, `ReadTrackedUser`, `DeleteUser`, or `UpdateUser`.
