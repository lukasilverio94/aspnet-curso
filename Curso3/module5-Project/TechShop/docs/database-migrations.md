# Database migrations (TechShop)

## Error: "There is already an object named 'Categories'"

`dotnet ef database update` runs pending migrations from `__EFMigrationsHistory`. If **`Categories` (or other tables) already exist** but **`20261106172938_InitialState` is not in that history**, EF tries to create them again and SQL Server fails.

### Option 1 — Development reset (simplest)

If you **do not need to keep** the current database data:

1. Drop the database (e.g. in SSMS: delete DB `TechShopDb`, or use Docker/sqlcmd).
2. Run:

```bash
dotnet ef database update
```

### Option 2 — Keep the database, schema already matches

Only use this if the database **already matches** what migration `InitialState` would create (Identity tables, `Categories`, `Products` with `Description`/`ImageUrl`, `Orders`, `OrderItems`, indexes, seed data).

1. Open SQL Server and run the script: `Scripts/MarkInitialMigrationApplied.sql` (read the warnings inside it first).
2. Run `dotnet ef database update` again; it should report **No pending migrations**.

If anything is missing (e.g. no `AspNetUsers`, no `Orders`), your DB is **not** aligned with `InitialState` — use Option 1 or fix the schema manually before marking the migration.

### Option 3 — New database name

Point `ConnectionStrings:DefaultConnection` at a **new** database name (e.g. `TechShopDb_v2`) so EF can create everything from scratch without touching the old file.

### Check current state

```sql
SELECT * FROM __EFMigrationsHistory;
SELECT name FROM sys.tables WHERE name IN ('Categories', 'Products', 'AspNetUsers', 'Orders', 'OrderItems')
ORDER BY name;
```
