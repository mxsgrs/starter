# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this project.

## Folder and file structure

Tests are organized under `Facts/` as follows:
- One folder per repository, named `{RepositoryName}TestCases` (e.g. `UserRepositoryTestCases`)
- One test class per repository method, named `{MethodName}Tests.cs` (e.g. `CreateUserTests.cs`)

## Facts

No XML doc comments above `[Fact]` methods.

## Test class structure

**IDisposable** - Each test class should implement the `IDisposable` interface and clean only the tables concerned by the test it contains.
`GC.SuppressFinalize(this);` is needed at the end of each Dispose. Place the `Dispose()` method after all facts, at the bottom of each test class.

```csharp
    public void Dispose()
    {
        using NetworkDbContext context = fixture.CreateDatabaseContext();
        context.Users.ExecuteDelete();
        GC.SuppressFinalize(this);
    }
```

