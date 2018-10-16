using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Paper.Media.Routing
{
  interface IVehicle { }
  class Car : IVehicle { }
  class Bus : IVehicle { }

  class Passenger
  {
    public string Name { get; set; }
    public IVehicle Vehicle { get; set; }

    public Passenger(string name, IVehicle vehicle)
    {
      this.Name = name;
      this.Vehicle = vehicle;
    }
  }
  
  public class DefaultObjectFactoryTest
  {
    [Fact]
    public void CreateInstance_ExactMatch_Test()
    {
      // Given
      var factory = new DefaultObjectFactory();
      var car = new Car();
      var bus = new Bus();
      factory.Add(car);             //<-- Mapeando o tipo
      factory.Add<IVehicle>(bus);   //<-- Mapeando a interface
      var passengerName = "Foo Bar";
      // When
      var passenger = factory.CreateInstance<Passenger>(passengerName);
      // Then
      var expected = new object[] { bus, "Foo Bar" };
      var obtained = new object[] { passenger.Vehicle, passenger.Name };
      Assert.Equal(expected, obtained);
    }

    [Fact]
    public void CreateInstance_InheritanceMatch_Test()
    {
      // Given
      var factory = new DefaultObjectFactory();
      var car = new Car();
      var bus = new Bus();
      factory.Add(car);   //<-- Mapeando o tipo
      factory.Add(bus);   //<-- Mapeando o tipo
      var passengerName = "Foo Bar";
      // When
      var passenger = factory.CreateInstance<Passenger>(passengerName);
      // Then
      var expected = new object[] { car, "Foo Bar" };
      var obtained = new object[] { passenger.Vehicle, passenger.Name };
      Assert.Equal(expected, obtained);
    }

    [Fact]
    public void CreateInstance_NotFound_Test()
    {
      // Given
      var factory = new DefaultObjectFactory();
      var passengerName = "Foo Bar";
      // When
      var passenger = factory.CreateInstance<Passenger>(passengerName);
      // Then
      var expected = "Foo Bar";
      var obtained = passenger.Name;
      Assert.Equal(expected, obtained);
      Assert.Null(passenger.Vehicle);
    }
  }
}