---
name: orleans-best-practices
description: "Microsoft Orleans best practices for this codebase. Use when writing or reviewing grain interfaces, grain implementations, serialization attributes, concurrency attributes, grain key strategies, storage providers, or tenant-scoped grain patterns. Use for [Immutable], [return: Immutable], [GenerateSerializer], [AlwaysInterleave], [OneWay], [StatelessWorker], [Reentrant], ITenantGrain, IWithTenantAccessor, TenantGrainKey, and StorageProvider patterns."
---

# Orleans Best Practices

## When to Use

- Writing a new grain interface or implementation
- Adding parameters or return values to grain methods
- Choosing a grain key type
- Selecting a grain base class
- Adding state persistence to a grain
- Making a grain tenant-aware

---

## 1. Serialization — `[Immutable]` and `[return: Immutable]`

### Rule

All collection and complex object parameters crossing grain boundaries **must** be decorated with `[Immutable]`. All methods returning collections or complex objects **must** use `[return: Immutable]`.

This tells Orleans the caller guarantees the object won't be mutated — Orleans skips copying the argument, avoiding large allocations on hot paths.

### On grain interfaces

```csharp
public interface IHeroGrain : IGrainWithStringKey, ITenantGrain
{
    [return: Immutable]
    Task<List<Hero>> GetAllAsync();

    [return: Immutable]
    Task<Hero?> GetByKeyAsync(string heroKey);
}
```

### Rules

- Use `[Immutable]` / `[return: Immutable]` on the **interface**, not the implementation
- The implementation omits the `[Immutable]` attribute; it receives the unwrapped type
- Scalar types (`string`, `int`, `bool`, enums) do **not** need `[Immutable]`
- ❌ Avoid the `Immutable<T>` struct wrapper — it changes the public API type and leaks Orleans internals into the interface signature. Prefer the attribute form at all times

---

## 2. Serialization — `[GenerateSerializer]` and `[Id(n)]`

All types sent over grain calls or stored in grain state must have Orleans serialization attributes.

```csharp
[GenerateSerializer]
public class HeroGrainState
{
    [Id(0)]
    public List<Hero> Heroes { get; set; } = [];
}

[GenerateSerializer]
public record MyResult(
    [property: Id(0)] string Value,
    [property: Id(1)] bool IsValid
);
```

### Rules

- `[Id(n)]` **must** start at `0` and be contiguous
- Never reuse or reorder `Id` values — this breaks deserialization of persisted state
- Use `[Alias("type-name")]` on grain interfaces to decouple the type name from the wire name (prevents breaking changes on rename)
- Records can use `[property: Id(n)]` on the primary constructor

---

## 3. Concurrency Attributes

### `[AlwaysInterleave]` — read-only, non-mutating methods

Bypasses the grain's single-threaded turn-based execution. Use for reads that do NOT mutate grain state:

```csharp
public interface IHeroGrain : IGrainWithStringKey, ITenantGrain
{
    [AlwaysInterleave]
    [return: Immutable]
    Task<List<Hero>> GetAllAsync();
}
```

### `[OneWay]` — fire-and-forget

No return value expected (Orleans won't wait for completion). Use for notification/side-effect methods:

```csharp
[OneWay]
Task NotifyUpdateAsync(string heroKey);
```

### `[Reentrant]` — on the grain class, not the interface

Allows the grain to process new messages while awaiting async operations:

```csharp
[Reentrant]
public class MyGrain : Grain, IMyGrain { }
```

> ⚠️ Never use `[Reentrant]` on grains with mutable state that require strict ordering.

### `[StatelessWorker]` — pooled, stateless workers

Any number of instances can be created per silo. No per-grain state. Use for CPU/IO work that scales out:

```csharp
[StatelessWorker]      // unlimited instances
[StatelessWorker(1)]   // 1 instance per silo (bounded)
public class WorkerGrain : Grain, IWorkerGrain { }
```

---

## 4. Grain Key Strategy

| Key Type | `IGrainWithXxxKey`     | When to Use                                                       |
| -------- | ---------------------- | ----------------------------------------------------------------- |
| Integer  | `IGrainWithIntegerKey` | Singleton or small cardinality; use `0` for the default singleton |
| String   | `IGrainWithStringKey`  | Per-entity, per-tenant, or semantic composite keys                |
| GUID     | `IGrainWithGuidKey`    | Globally unique instance identity                                 |

### Tenant-scoped String Keys

For tenant-aware grains use `TenantGrainKey.Create(...)`. Two formats are supported:

| Format                         | Method                        | When to Use                                           |
| ------------------------------ | ----------------------------- | ----------------------------------------------------- |
| `tenant/{tenantKey}/{grainId}` | `Create(tenantKey, grainKey)` | Multiple grain instances per tenant (e.g. per-entity) |
| `tenant/{tenantKey}`           | `Create(tenantKey)`           | One grain instance per tenant (singleton per tenant)  |

The `tenant/` prefix prevents ambiguous parsing when the tenant or grain key itself contains `/`.

```csharp
// Grain-specific key (multiple instances per tenant)
var key = TenantGrainKey.Create("tenant-a", "heroes");
// → "tenant/tenant-a/heroes"

// Tenant-only key (one grain instance per tenant)
var key = TenantGrainKey.Create("tenant-a");
// → "tenant/tenant-a"

// Resolving a grain
grainFactory.GetGrain<IHeroGrain>(TenantGrainKey.Create(tenantKey, "heroes"));

// Parsing inside a grain — GetTenantKey/GetGrainKey throw FormatException on bad format
var primaryKey = this.GetPrimaryKeyString();
var tenantKey = TenantGrainKey.GetTenantKey(primaryKey);
var grainKey  = TenantGrainKey.GetGrainKey(primaryKey); // empty string for tenant-only keys

// Safe parsing — TryParse returns a TenantGrainKey record struct (not out string? params)
if (TenantGrainKey.TryParse(primaryKey, out var parsed))
{
    // parsed.TenantKey, parsed.GrainKey
    // parsed.IsTenantOnly — true when no grain segment
}

// Zero-allocation path from Orleans grain identity (IdSpan → UTF-8 bytes, no ToString())
if (TenantGrainKey.TryParse(context.GrainId.Key, out var parsed))
{
    // parsed.TenantKey, parsed.GrainKey
}
```

### Rules

- Always use `TenantGrainKey.Create(...)` to build tenant-scoped keys — never concatenate manually
- Parse via `TenantGrainKey.GetTenantKey` / `GetGrainKey` (throws) or `TryParse` (safe) — never split on `/` manually
- `TryParse` accepts `string`, `ReadOnlySpan<char>`, `ReadOnlySpan<byte>`, and `IdSpan` — prefer `IdSpan` in activators to avoid `.ToString()` allocation
- Prefer `TryParse` in grain logic; reserve the throwing overloads for middleware/filters where an invalid key is always a bug

---

## 5. Base Class Selection

Always use `Grain` (stateless base). Add state via `IPersistentState<TState>` constructor injection — **not** `Grain<TState>` (legacy).

| Class           | State    | Use When                              |
| --------------- | -------- | ------------------------------------- |
| `Grain`         | None     | All grains — stateless or stateful    |
| `Grain<TState>` | Built-in | ❌ Legacy. Do **not** use in new code |

```csharp
// Stateless grain
public sealed class WorkerGrain : Grain, IWorkerGrain { }

// Stateful grain — inject IPersistentState<T>
public sealed class HeroGrain : Grain, IHeroGrain
{
    private readonly IPersistentState<HeroGrainState> _state;

    public HeroGrain(
      [PersistentState("heroes", "heroes")]
      IPersistentState<HeroGrainState> state
    )
    {
        _state = state;
    }
}
```

---

## 6. Grain Persistence

Inject `IPersistentState<TState>` into the constructor with `[PersistentState("stateName", "providerName")]`. This is the **recommended** approach in Orleans 10. `Grain<TState>` is considered legacy and should not be used in new code.

```csharp
public sealed class HeroGrain : Grain, IHeroGrain, IWithTenantAccessor<AppTenant>
{
    private readonly IPersistentState<HeroGrainState> _state;

    public HeroGrain(
      [PersistentState("heroes", "heroes")]
      IPersistentState<HeroGrainState> state
    )
    {
        _state = state;
    }

    // State is loaded before OnActivateAsync() — safe to read in any method
    public Task<List<Hero>> GetAllAsync() => Task.FromResult(_state.State.Heroes);

    public async Task SetHeroesAsync(List<Hero> heroes)
    {
        _state.State.Heroes = heroes;
        await _state.WriteStateAsync();
    }
}
```

### Rules

- `_state.State` is loaded before `OnActivateAsync()` — safe to read immediately; **do not** access it in the constructor
- Always call `_state.WriteStateAsync()` after mutating `_state.State`
- The first argument to `[PersistentState]` is the **state name** (logical); the second is the **storage provider name**
- Multiple independent states are supported — inject multiple `IPersistentState<T>` parameters with distinct state names
- Provider names are configured in the silo builder (e.g. `siloBuilder.AddMemoryGrainStorage("heroes")`)

---

## 7. Tenant-Aware Grains

### `ITenantGrain`

Implement `ITenantGrain` on any grain whose primary key embeds a tenant key:

```csharp
public interface IHeroGrain : IGrainWithStringKey, ITenantGrain
{
    [return: Immutable]
    Task<List<Hero>> GetAllAsync();
}
```

### `IWithTenantAccessor<TTenant>` — receiving tenant context from the grain activator

The `TenantGrainActivator<TTenant>` automatically populates the grain's `TenantAccessor.Tenant` when the grain is activated — but only when the grain implements `IWithTenantAccessor<TTenant>`. Add a public auto-property of type `TenantAccessor<TTenant>` to opt in:

```csharp
public sealed class HeroGrain : Grain, IHeroGrain, IWithTenantAccessor<AppTenant>
{
    private readonly IPersistentState<HeroGrainState> _state;

    public HeroGrain(
      [PersistentState("heroes", "heroes")]
      IPersistentState<HeroGrainState> state
    )
    {
        _state = state;
    }

    public TenantAccessor<AppTenant> TenantAccessor { get; } = new();
}
```

### Propagating tenant into a DI scope

Inject tenant-aware services directly via the constructor. `TenantGrainActivator<TTenant>` sets up a tenant-scoped DI context before the grain is constructed — injected services are already resolved for the correct tenant. No `IServiceScopeFactory` or manual tenant propagation is needed:

```csharp
public sealed class HeroGrain : Grain, IHeroGrain
{
    private readonly IHeroDataClient _heroDataClient;
    private readonly IPersistentState<HeroGrainState> _state;

    public HeroGrain(
        IHeroDataClient heroDataClient,
        [PersistentState("heroes", "heroes")]
        IPersistentState<HeroGrainState> state
    )
    {
        _heroDataClient = heroDataClient;
        _state = state;
    }

    private async Task EnsureHeroesAsync()
    {
        if (_state.State.Heroes.Count > 0)
            return;

        _state.State.Heroes = await _heroDataClient.GetAll();
        await _state.WriteStateAsync();
    }
}
```

> `IWithTenantAccessor<TTenant>` is only needed when the grain itself must read the tenant identity (e.g. for logging or routing). For pure data access through a proxied service, constructor injection alone is sufficient.

---

## 8. Silo Registration

Register the call filter via `UseMultitenancy<TTenant>()` on the silo builder. Place this alongside other silo configuration:

```csharp
siloBuilder.UseMultitenancy<AppTenant>();
```

This registers `ITenantOrleansResolver<TTenant>` and `TenantGrainActivator<TTenant>`. The activator sets tenant context once per grain activation.

---

## Quick Reference Checklist

When writing a new tenant-aware grain:

- [ ] Interface extends `IGrainWithStringKey` + `ITenantGrain`
- [ ] Grain class implements `IWithTenantAccessor<TTenant>` with a `TenantAccessor<TTenant>` property — **only if** the grain needs to read the tenant identity directly (e.g. logging, routing); omit for grains that only use injected tenant-aware services
- [ ] Collections and complex types returned from interface methods use `[return: Immutable]`
- [ ] Complex/collection parameters use `[Immutable]`
- [ ] Read-only methods use `[AlwaysInterleave]`
- [ ] State type has `[GenerateSerializer]` + `[Id(n)]` starting at `0` on every property
- [ ] Stateful grain injects `IPersistentState<TState>` via `[PersistentState("stateName", "providerName")]` constructor parameter; grain class extends `Grain` (not `Grain<TState>`)
- [ ] Grain keys are built with `TenantGrainKey.Create(tenantKey, grainKey)` — never manual string concat
- [ ] Silo builder calls `UseMultitenancy<TTenant>()`
