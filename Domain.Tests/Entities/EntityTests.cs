using Domain.Entities;
using Domain.Events;
using Xunit;

namespace Domain.Tests.Entities;

public class EntityTests
{
    private class TestEntity : Entity
    {
        public TestEntity(Guid id) : base(id)
        {
        }

        public void AddTestEvent(Event @event)
        {
            AddEvent(@event);
        }
    }

    private class TestEvent : Event
    {
    }

    [Fact]
    public void Constructor_ShouldInitializeId()
    {
        // Arrange
        Guid expectedId = Guid.NewGuid();

        // Act
        TestEntity entity = new TestEntity(expectedId);

        // Assert
        Assert.Equal(expectedId, entity.Id);
    }

    [Fact]
    public void DomainEvents_ShouldBeEmptyOnCreation()
    {
        // Arrange
        TestEntity entity = new TestEntity(Guid.NewGuid());

        // Act & Assert
        Assert.Empty(entity.Events);
    }

    [Fact]
    public void AddEvent_ShouldAddEventToDomainEvents()
    {
        // Arrange
        TestEntity entity = new TestEntity(Guid.NewGuid());
        TestEvent testEvent = new TestEvent();

        // Act
        entity.AddTestEvent(testEvent);

        // Assert
        Assert.Single(entity.Events);
        Assert.Contains(testEvent, entity.Events);
    }

    [Fact]
    public void ClearDomainEvents_ShouldRemoveAllEvents()
    {
        // Arrange
        TestEntity entity = new TestEntity(Guid.NewGuid());
        entity.AddTestEvent(new TestEvent());
        entity.AddTestEvent(new TestEvent());

        // Act
        entity.ClearDomainEvents();

        // Assert
        Assert.Empty(entity.Events);
    }

    [Fact]
    public void DomainEvents_ShouldReturnReadOnlyCollection()
    {
        // Arrange
        TestEntity entity = new TestEntity(Guid.NewGuid());
        
        // Act
        IReadOnlyCollection<Event> events = entity.Events;
        
        // Assert
        Assert.IsAssignableFrom<IReadOnlyCollection<Event>>(events);
    }

    [Fact]
    public void AddMultipleEvents_ShouldMaintainOrder()
    {
        // Arrange
        TestEntity entity = new TestEntity(Guid.NewGuid());
        TestEvent firstEvent = new TestEvent();
        TestEvent secondEvent = new TestEvent();

        // Act
        entity.AddTestEvent(firstEvent);
        entity.AddTestEvent(secondEvent);

        // Assert
        Assert.Equal(2, entity.Events.Count);
        Assert.Equal(firstEvent, entity.Events.First());
        Assert.Equal(secondEvent, entity.Events.Last());
    }
}