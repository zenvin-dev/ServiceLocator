This package contains a ServiceLocator pattern implementation, including utility for use with Unity.

Author's note from August 2025: \
I am currently [dogfooding](https://en.wikipedia.org/wiki/Eating_your_own_dog_food) this in my own projects, so any obvious bugs should be found quickly.

# Table of contents
- [Features](#features)
- [Installation](#installation)
- [Concepts / Glossary](#concepts--glossary)
- [Service Lifecycle](#service-lifecycle)
- [Usage](#usage)
    - [Service Locator Initialization](#initialization)
    - [Scope Setup](#scope-setup)
    - [Scope Removal](#scope-removal)
    - [Service Locator Disposal](#service-locator-disposal)
    - [Retrieving Services](#service-instance-retrieval)
    - [Code Generation](#code-generation)
    - [Events](#events)
- [Examples](#examples)


# Features
- Easy to understand and set up
- No dependencies on Unity's play mode lifecycle or project setup
- Different register modes for services (see [Concepts](#concepts--glossary))
- Scoped service registration
- Simple service lifecycle hooks through interfaces (see [Service Lifecycle](#service-lifecycle))
- Attribute-controls source generation for ease of use (see [Code Generation](#code-generation))
- Deferred service location through events
- Easy debugging through purpose-built Editor Window

# Installation
This package can be installed through the Unity Package Manager using the following URL:
```
https://github.com/zenvin-dev/ServiceLocator.git
```
To install a specific release, append `#release-name` to the URL. \
(See the [Unity documentation](https://docs.unity3d.com/2022.1/Documentation/Manual/upm-git.html#Git-GIT) for more info)


# Concepts / Glossary
**Service Locator** \
The center point of the system, which manages Scopes and handles retrieving service instances.

**Scope** \
A collection of Services. \
Scopes are always immutable after their creation. Aside from the Global Scope, each Scope is identified with a key to allow separating Services into different contexts.

**Global Scope** \
A special Scope created during initialization of the Service Locator.

**Default Scope** \
A Scope that is determined using the key returned by the `IScopeContextProvider` passed to the Service Locator during initialization.

**Service** \
An instance of an arbitrary type, registered inside a Scope. Each registered Service is identified by a contract type, which also needs to be used to retrieve it. \
Registration can happen as a Singleton, a Lazy instance, or through a Factory.

**Singleton** \
A Service that has been registered with a scope directly as an instance of an object. A reference to the instance will live inside that scope and be returned if a Service for the corresponding contract type is queried.

**Lazy instance** \
A mix between a Singleton and a Factory, Lazy instances are Services that were registered to a Scope through an instantiation callback. In contrast to a Factory however, Lazy instances will only invoke that callback once when the Scope is first queried with the corresponding contract type, and then always return the same result when queried again.

**Factory** \
A Service that is internally represented by an instantiation callback. Each time a Scope is queried with the corresponding contract type, the callback will be invoked and its result returned as the Service instance.


# Service Lifecycle
Service references are kept as long as the Scope containing them exists. \
Whenever a Scope is removed, the references will be removed together with the scope.

Services registered as Singletons may receive callbacks whenever they are registered with a Scope, or when their containing Scope is removed:
- If a Singleton instance implements `IInitializable`, its implementation of `IInitializable.Initialize(IScopeKey)` will be called whenever a containing Scope is built and added to the Service Locator.<br>The value of the method's parameter will be the key that the Scope was created for, or `null` if the Service was registered with the Global Scope.
- If a Singleton instance implements `IDisposable`, its implementation of `IDisposable.Dispose()` will be called when the containing Scope is removed.


# Usage
## Initialization
To use the `ServiceLocator`, it first needs to be initialized. This does **not** happen automatically, as to not mandate any specific project structure.

### Simple Initialization
Performing this initialization is as simple as calling
```csharp
ServiceLocator.Initialize();
```
at any point during your game's or application's lifecycle (though the recommendation would be to do it during initialization).

### Populating the Global Scope
The initialization always creates a *Global Scope*. To populate it with services, pass a build callback (`BuildServiceScopeCallback`) into `Initialize()` as an argument:
```csharp
ServiceLocator.Initialize(BuildGlobalScope);

static void BuildGlobalScope(ServiceScopeBuilder builder)
{
    builder.RegisterInstance("Hello World!");   // Registers the string "Hello World!" as a singleton instance with "System.String" as its contract type
}
```

### Advanced Initialization
The `Initialize()` method always returns a `FluentConfigurator` instance. This can be used to provide the `ServiceLocator` with a `IScopeContextProvider`. It may be possible to configure further values in the future.
```csharp
ServiceLocator
    .Initialize(...)
    .WithScopeContextProvider(new MyScopeContextProvider());
```

## Scope Setup
The `ServiceLocator` can contain an arbitrary number of keyed scopes. Those are created by calling 
```csharp
ServiceLocator.AddScope(...);
```
This method takes two arguments: 
- A key (`IScopeKey`) by which the new Scope will be identified, and 
- A build callback (`BuildServiceScopeCallback`), which is responsible for populating the new Scope with Services.

If either of those arguments are `null`, no new scope will be added. \
Likewise if the given key already had a scope associated with it.

### Parented Scope Setup
Scopes can be assigned a parent, to influence the behaviour of the `ServiceLocator` when attempting to retrieve a Service. \
To add a parent to a Scope, use the `SetParent()` method on the `ServiceScopeBuilder` passed to the build callback.

> [!NOTE]
> The Global Scope **can not** have a parent, and attempting to assign one will only result in a warning.

When creating a parent relationship, it can be constrained in three different ways that influence the lifecycle of the Scope that is being built:
- **Loose**: The relationship is merely a suggestion. If a Scope with the given parent key exists, it will be considered during Service retrieval. If it does not exist, that is fine too. **This is the default behaviour.**
- **Required**: If no Scope with the given parent key exists yet, the new Scope will not be built. If a Scope with the parent key *does* exist, the new Scope will be removed if its parent is removed.
- **Hardened**: Behaves like *Loose*, until a Scope with the given parent key exists. From that point onward, the relationship will be considered *Required*.

## Scope Removal
If a Scope is no longer needed, it can be removed from the Service Locator using its key:
```csharp
IScopeKey key = <some key>;
ServiceLocator.RemoveScope(key);
```
This will dispose and remove the Scope with the given key, as well as any Scopes that had required it as a parent (see above).

## Service Locator Disposal
Resetting the Service Locator to its uninitialized state should not usually be necessary in built applications or games. But the option exists nonetheless:
```csharp
ServiceLocator.Dispose();
```
Calling this method will dispose all Scopes (including the Global Scope) and remove them from the Service Locator.
> [!NOTE]
> This method will be called automatically inside Unity upon exiting play mode. \
> Otherwise, the static values inside the `ServiceLocator` would carry over into edit mode and persist until a domain reload happens.


## Service Instance Retrieval
Accessing Service instances always happens through the `ServiceLocator` class. \
For this purpose, it has two methods: `Get()` and `TryGet()`. The string singleton added in [Populating the Global Scope](#populating-the-global-scope) for example, could be retrieved like this:
```csharp
var myString = ServiceLocator.Get<string>();
```
or like this:
```csharp
var myString = ServiceLocator.TryGet(out string value) ? value : "fallback";
```

Both of those methods have a number of overloads to influence their behaviour. They are described below, but checking the inline documentation in the code is still recommended.

Internally, both `Get` and `TryGet` work almost the same. \
Externally, the difference is that `Get` can throw a `ServiceException` if no Service was found for the query, whereas `TryGet` will simply return `false`.

| Parameter                  | Type      | Effect                                  | Optional |
|:---------------------------|-----------|-----------------------------------------|-|
| `TInstance`                | Generic   | The instance type of the service that is expected to be returned by the query. | No |
| `TContract` | Generic | The type that the Service was registered with. Must only be supplied if the contract type is not the instance type. | Yes |
| `scope` | `IScopeKey` | The key of the scope in which to start looking for the Service instance. | Yes |
| `fallbackToGlobalScope` | `bool` | Only if `scope` is supplied:<br>If set to `true`, the global scope will be queried as well, if no Service instance was found in keyed Scope or any of its parents. | Yes |
| `required` | `bool` | Only on `Get()`:<br> If `true`, the method will return `default(TInstance)` instead of throwing an exception, if no Service was found for the query. | Yes |
| out `instance` | `TInstance` | Only on `TryGet()`:<br>Contains the Service instance returned by the query, or `default(TInstance)` if no instance was found for the query. | No |

Queries for a Service instance always follow the same procedure: \
If no `scope` was provided, the Default Scope will be used as a starting point. If there is no Default Scope, the Global Scope will be used as a starting point. \
Once a starting point is defined, it and all its parents will be queried for a Service instance in ascending order, until there is either no parent left, the query was successful, or there is a circular reference among the parents. \
If all parents have been queried without success, the query will either be considered unsuccessful, or the Global Scope will be queried as a last resort (depending on whether `fallbackToGlobalScope` is set to `true`, which is the default).

## Code Generation
For ease of use, the package contains a Roslyn Source Generator that can trivialize service location. To do this, it will generate a method that calls `ServiceLocator.Get` for any relevant field or property in the class.

To generate that method for any given class, the following criteria need to be fulfilled:
- The class needs to be decorated with the `[InjectServices]` attribute
- The class needs to be `partial`
- The class needs to contain at least one field or property decorated with the `[InjectService]` (singular) attribute

The generated method will always be named `InjectServices__` and take two optional arguments: an `IScopeKey` for the scope that should be queried, and a `bool` to pass through to `fallbackToGlobalScope`. \
It will also automatically call implementations generated for any base classes, if present. This way, it is able to handle even `private` members.

## Events
The Service Locator exposes a number of events through its `Events` property. Those can be used to defer retrieving Services until either a Scope has been initialized. \
Refer to inline documentation for a description on individual events.


# Examples
Below are some (Unity-specific) examples on how the Service Locator is intended to be used.

## Simple Service Locator setup and Service location
```csharp
public class EntryPoint : MonoBehaviour
{
    // Awake is run before Start. 
    // This is important to consider, because otherwise the initialization might happen after the call to Get().
    private void Awake()
    {
        ServiceLocator.Initialize(BuildGlobalScope);
    }

    private static void BuildGlobalScope(ServiceScopeBuilder builder)
    {
        // Create a primitive Sphere GameObject and registers it as a singleton
        builder.RegisterSingleton(GameObject.CreatePrimitive(PrimitiveType.Sphere));
    }
}

public class Consumer : MonoBehaviour
{
    private GameObject sphere;

    private void Start()
    {
        sphere = ServiceLocator.Get<GameObject>();  // retrieves the GameObject created above
    }
}
```

## Simple Service Locator Setup and Service location using Source Generation
Consider the example above. However, the `Consumer` will be changed as follows:
```csharp
[InjectServices]
public partial class Consumer : MonoBehaviour
{
    // Make "sphere" optional, so there is no error when the service cannot be found
    [InjectService(false)] private GameObject sphere;
    // Use "IDisposable" as contract type
    [InjectService(typeof(IDisposable))] private Stream stream;

    private void Start()
    {
        // Instead of assigning the values manually, the generated method will do it for you
        InjectServices__();
    }
}
```

## Using Scenes as a scope keys
Some Services will only be available to specific scenes. So it makes sense to create a Scope for each relevant scene. \
This can be achieved by creating an implementation of `IScopeKey` that looks at the current scene:
```csharp
public readonly struct SceneKey : IScopeKey
{
    private readonly int buildIndex;


    public SceneKey(Scene scene)
    {
        buildIndex = scene.buildIndex;
    }


    public bool Equals(IScopeKey key)
    {
        return key is SceneKey sceneKey && sceneKey.buildIndex == buildIndex;
    }
}
```
This can then be used when creating a new Scope:
```csharp
private void Awake()
{
    var key = new SceneKey(gameObject.scene.buildIndex);
    // Note that this assumes that the ServiceLocator has been initialized previously
    ServiceLocator.AddScope(key, BuildSceneScope);
}

private static void BuildSceneScope(ServiceScopeBuilder builder)
{
    // ...
}
```

> [!NOTE]
> This approach will only work reliably in projects using Unity's built-in `SceneManager`. When loading scenes through `Addressables`, looking at the build index may not be sufficient.
