# <img src="https://raw.githubusercontent.com/TrackableEntities/TrackableEntities.Core/develop/TrackableEntities.EF.Core/icon.png" width="32" height="32" /> Trackable Entities for Entity Framework Core

[![NuGet version](https://badge.fury.io/nu/TrackableEntities.EF.Core.svg)](https://badge.fury.io/nu/TrackableEntities.EF.Core)
[![Downloads](https://img.shields.io/nuget/dt/TrackableEntities.EF.Core.svg?logo=nuget&color=green)](https://www.nuget.org/packages/TrackableEntities.EF.Core) 
[![Build](https://github.com/TrackableEntities/TrackableEntities.Core/actions/workflows/deploy.yml/badge.svg?branch=develop)](https://github.com/TrackableEntities/TrackableEntities.Core/actions/workflows/deploy.yml)

Change-tracking across service boundaries with [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/) Web API and [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/).

## What is Trackable Entities?

[Trackable Entities](http://trackableentities.github.io/) allows you to mark client-side entities as Added, Modified or Deleted, so that _entire object graphs_ can be sent to a service where changes can be saved with a single round trip to the server and within a single transaction.

## Installation

Trackable Entities for EF Core 8 is available as a NuGet package that can be installed in an ASP.NET Core Web API project that uses Entity Framework Core.

You can use the [Package Manager UI or Console](https://docs.microsoft.com/en-us/nuget/tools/package-manager-console) in Visual Studio to install the TE package.

```bash
install-package TrackableEntities.EF.Core
```

You can also use the [.NET Core CLI](https://docs.microsoft.com/en-us/dotnet/core/tools/) to install the TE package.

```bash
dotnet add package TrackableEntities.EF.Core
```

## Packages for Previous Versions of EntityFramework Core

##### [EntityFramework Core v7](https://www.nuget.org/packages/TrackableEntities.EF.Core/7.0.0) | [EntityFramework Core v6](https://www.nuget.org/packages/TrackableEntities.EF.Core/6.0.0) | [EntityFramework Core v5](https://www.nuget.org/packages/TrackableEntities.EF.Core/5.0.1) | [EntityFramework Core v3](https://www.nuget.org/packages/TrackableEntities.EF.Core/3.1.1)


## Trackable Entities Interfaces

The way Trackable Entities allows change-tracking across service boundaries is by adding a `TrackingState` property to each entity. `TrackingState` is a simple enum with values for `Unchanged`, `Added`, `Modified` and `Deleted`.  The TE library will traverse objects graphs to read this property and inform EF Core of each entity state, so that you can save all the changes at once, wrapping them in a single transaction that will roll back if any individual update fails.

`TrackingState` is a member of the `ITrackable` interface, which also includes a `ModifiedProperties` with the names of all entity properties that have been modified, so that EF Core can perform partial entity updates.

In order for clients to merge changed entities back into client-side object graphs, TE includes an `IMergeable` interface that has an `EntityIdentifier` property for correlating updated entities with original entities on the client.  The reason for returning updated entities to the client is to transmit database-generated values for things like primary keys or concurrency tokens.

Each tracked entity needs to implement `ITrackable`, as well as `IMergeable`, either directly or in a base class. In addition, properties of these interfaces should be decorated with a `[NotMapped]` attribute so that EF Core will not attempt to save these to the database.  For example, a `Product` entity might look like this:

```csharp
public class Product : ITrackable, IMergeable
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal? UnitPrice { get; set; }

    [NotMapped]
    public TrackingState TrackingState { get; set; }

    [NotMapped]
    public ICollection<string> ModifiedProperties { get; set; }

    [NotMapped]
    public Guid EntityIdentifier { get; set; }
}
```

Server-side trackable entities can either be writen by hand or generated from an existing database using code-generation techniques. Trackable Entities provides [CodeTemplates](https://www.nuget.org/packages?q=TrackableEntities.CodeTemplates) NuGet packages which add T4 templates to a traditional .NET class library for generating trackable entities using the Visual Studio wizard for adding an ADO.NET Entity Data Model. You can then add a NetStandard class library which links to the generated entities. See the [TE Core Sample](https://github.com/TrackableEntities/TrackableEntities.Core.Sample) project for an example of how to do this.

> **Note**: The reason for the extra project with generated entities is that EF Core does not yet support _customizing_ scaffolded models for server-side entities. We will provide code generation packages for Trackable Entities as soon as EF Core tooling is updated to support customization.

## Usage

For an example of how to use Trackable Entities for EF Core in an ASP.NET Core Web API, have a look at [OrderContoller](https://github.com/TrackableEntities/TrackableEntities.Core.Sample/blob/master/NetCoreSample.Web/Controllers/OrderController.cs) in the sample app, which includes GET, POST, PUT and DELETE actions.  GET actions don't inlude any code that uses Trackable Entities, but the other actions set `TrackingState` before calling `ApplyChanges` on the `DbContext` class and then saving changes.

```csharp
// Set state to added
order.TrackingState = TrackingState.Added;

// Apply changes to context
_context.ApplyChanges(order);

// Persist changes
await _context.SaveChangesAsync();
```

After saving changes, `LoadRelatedEntitiesAsync` is called in order to populate reference properties for foreign keys that have been set. This is required, for example, in order to set the `Customer` property of an `Order` that has been added with a specified `CustomerId`. This way the client can create a new `Order` without populating the `Customer` property, which results in a smaller payload when sending the a new order to the Web API.  Loading related entities then populates the ` Customer` property before returning the added order to the client.

```csharp
// Populate reference properties
await _context.LoadRelatedEntitiesAsync(order);
```

Lastly, you should call `AcceptChanges` to reset `TrackingState` on each entity in the object graph before returning it to the client, so that the client can then make changes to the object and send it back to the Web API for persisting those changes.

```csharp
// Reset tracking state to unchanged
_context.AcceptChanges(order);
```

## Questions and Feedback

If you have any questions about [Trackable Entities](http://trackableentities.github.io/), would like to request features, or discover any bugs, please create an [issue](https://github.com/TrackableEntities/TrackableEntities.Core/issues) on the GitHub repository.  If you wish to [contribute](http://trackableentities.github.io/6-contributing.html) to Trackable Entities, pull [requests](https://help.github.com/articles/about-pull-requests/) are welcome!
