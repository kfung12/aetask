using ae_service_ship.Controllers;
using ae_service_ship.Models;
using ae_service_ship.Repositories;
using ae_service_ship.ViewModels;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ae_service_ship.UnitTests
{
    public class ShipsControllerTests
    {
        [Fact]
        public async Task GetShipsAsync_ReturnsExpectedResult()
        {
            //setup
            var repository = new Mock<IShipsRepository>();
            var expected = new List<Ship>();
            expected.Add(CreateShip(1));
            expected.Add(CreateShip(2));
            expected.Add(CreateShip(3));
            expected.Add(CreateShip(4));
            expected.Add(CreateShip(5));
            repository.Setup(repo => repo.GetShipsAsync()).ReturnsAsync(expected);
            var controller = new ShipsController(repository.Object);

            //test
            var result = await controller.GetShipsAsync();

            result.Value.Should().BeEquivalentTo(expected, options => options.ComparingByMembers<Ship>());
        }

        [Fact]
        public async Task GetShipsAsync_ReturnsEmptyResult()
        {
            //setup
            var repository = new Mock<IShipsRepository>();
            repository.Setup(repo => repo.GetShipsAsync()).ReturnsAsync(new List<Ship>());
            var controller = new ShipsController(repository.Object);

            //test
            var result = await controller.GetShipsAsync();

            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task GetShipAsync_ReturnsExpectedResult()
        {
            //setup
            var repository = new Mock<IShipsRepository>();
            var expected = CreateShip(1);
            repository.Setup(repo => repo.GetShipAsync(It.IsAny<long>())).ReturnsAsync(expected);
            var controller = new ShipsController(repository.Object);

            //test
            var result = await controller.GetShipAsync(1);

            result.Value.Should().BeEquivalentTo(expected, options => options.ComparingByMembers<Ship>());
        }

        [Fact]
        public async Task GetShipAsync_ReturnsNotFound()
        {
            //setup
            var repository = new Mock<IShipsRepository>();
            repository.Setup(repo => repo.GetShipAsync(It.IsAny<long>())).ReturnsAsync((Ship)null);
            var controller = new ShipsController(repository.Object);

            //test
            var result = await controller.GetShipAsync(1);

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CreateShipAsync_ReturnsItem()
        {
            //setup
            var repository = new Mock<IShipsRepository>();
            repository.Setup(repo => repo.GetNewId()).ReturnsAsync(1);
            Random rnd = new Random();
            var shipToCreate = new ShipDto
            {
                Id = 1,
                Name = "Ship123",
                Lat = (decimal)rnd.Next(-9000, 9000) / 100,
                Long = (decimal)rnd.Next(-18000, 18000) / 100,
                Velocity = 100
            };
            var controller = new ShipsController(repository.Object);

            //test
            var result = await controller.CreateShipAsync(shipToCreate);
            var resultValue = ((CreatedAtActionResult)result.Result).Value as ShipDto;
            resultValue.Should().BeEquivalentTo(shipToCreate, options => options.ComparingByMembers<ShipDto>().ExcludingMissingMembers());
        }

        [Fact]
        public async Task UpdateShipAsync_SuccessAndReturnsNoContent()
        {
            //setup
            var repository = new Mock<IShipsRepository>();
            var expected = CreateShip(1);
            repository.Setup(repo => repo.GetShipAsync(It.IsAny<long>())).ReturnsAsync(expected);

            var shipToUpdate = new ShipDto
            {
                Id = expected.Id,
                Name = expected.Name + "Update",
                Lat = expected.Lat,
                Long = expected.Long,
                Velocity = expected.Velocity + 50
            };
            var controller = new ShipsController(repository.Object);

            //test
            var result = await controller.UpdateShipAsync(shipToUpdate.Id, shipToUpdate);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdateShipAsync_ReturnsBadRequest()
        {
            //setup
            var repository = new Mock<IShipsRepository>();
            var expected = CreateShip(1);
            repository.Setup(repo => repo.GetShipAsync(It.IsAny<long>())).ReturnsAsync(expected);

            var shipToUpdate = new ShipDto
            {
                Id = expected.Id,
                Name = expected.Name + "Update",
                Lat = expected.Lat,
                Long = expected.Long,
                Velocity = expected.Velocity + 50
            };
            var controller = new ShipsController(repository.Object);

            //test
            var result = await controller.UpdateShipAsync(shipToUpdate.Id + 1, shipToUpdate);

            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task UpdateShipAsync_ReturnsNotFound()
        {
            //setup
            var repository = new Mock<IShipsRepository>();
            var expected = CreateShip(1);
            repository.Setup(repo => repo.GetShipAsync(It.IsAny<long>())).ReturnsAsync((Ship)null);

            var shipToUpdate = new ShipDto
            {
                Id = expected.Id,
                Name = expected.Name + "Update",
                Lat = expected.Lat,
                Long = expected.Long,
                Velocity = expected.Velocity + 50
            };
            var controller = new ShipsController(repository.Object);

            //test
            var result = await controller.UpdateShipAsync(shipToUpdate.Id, shipToUpdate);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetClosestPortAsync_ReturnsExpectedResult()
        {
            //setup
            var repository = new Mock<IShipsRepository>();
            Random rnd = new Random();
            var port = CreatePort(1);
            var ship = CreateShip(1);
            ship.Velocity = rnd.Next(1, 1000);
            var distance = rnd.Next(1, 1000);
            var expected = new PortInfoDto
            {
                Port = new PortDto
                {
                    Name = "Port1",
                    Lat = 0,
                    Long = 0
                },
                Distance = distance,
                ETA = distance / ship.Velocity.Value
            };
            repository.Setup(repo => repo.GetShipAsync(It.IsAny<long>())).ReturnsAsync(ship);
            repository.Setup(repo => repo.GetClosestPortAsync(It.IsAny<Ship>())).ReturnsAsync(expected);

            var controller = new ShipsController(repository.Object);

            //test
            var result = await controller.GetClosestPortAsync(1);

            result.Value.Should().BeEquivalentTo(expected, options => options.ComparingByMembers<PortInfoDto>());
        }

        [Fact]
        public async Task GetClosestPortAsync_ReturnsNotFound()
        {
            //setup
            var repository = new Mock<IShipsRepository>();
            repository.Setup(repo => repo.GetShipAsync(It.IsAny<long>())).ReturnsAsync((Ship)null);

            var controller = new ShipsController(repository.Object);

            //test
            var result = await controller.GetClosestPortAsync(1);

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetClosestPortAsync_MissingVelocityReturns500Error()
        {
            //setup
            var repository = new Mock<IShipsRepository>();
            Random rnd = new Random();
            var port = CreatePort(1);
            var ship = CreateShip(1);
            var distance = rnd.Next(1, 1000);
            var expected = new PortInfoDto
            {
                Port = new PortDto
                {
                    Name = "Port1",
                    Lat = 0,
                    Long = 0
                },
                Distance = distance,
                ETA = rnd.Next(1, 1000)
            };
            repository.Setup(repo => repo.GetShipAsync(It.IsAny<long>())).ReturnsAsync(ship);
            repository.Setup(repo => repo.GetClosestPortAsync(It.IsAny<Ship>())).ReturnsAsync(expected);

            var controller = new ShipsController(repository.Object);

            //test
            var result = await controller.GetClosestPortAsync(1);

            result.Result.Should().BeOfType<ObjectResult>();
            var resultResult = (ObjectResult)result.Result;
            //resultResult.Value
            resultResult.StatusCode.Should().Equals(StatusCodes.Status500InternalServerError);
            resultResult.Value.Should().BeEquivalentTo("Velocity information is missing or v<=0");
        }

        [Fact]
        public async Task GetClosestPortAsync_NegativeVelocityReturns500Error()
        {
            //setup
            var repository = new Mock<IShipsRepository>();
            Random rnd = new Random();
            var port = CreatePort(1);
            var ship = CreateShip(1);
            ship.Velocity = -50;
            var distance = rnd.Next(1, 1000);
            var expected = new PortInfoDto
            {
                Port = new PortDto
                {
                    Name = "Port1",
                    Lat = 0,
                    Long = 0
                },
                Distance = distance,
                ETA = distance / ship.Velocity.Value
            };
            repository.Setup(repo => repo.GetShipAsync(It.IsAny<long>())).ReturnsAsync(ship);
            repository.Setup(repo => repo.GetClosestPortAsync(It.IsAny<Ship>())).ReturnsAsync(expected);

            var controller = new ShipsController(repository.Object);

            //test
            var result = await controller.GetClosestPortAsync(1);

            result.Result.Should().BeOfType<ObjectResult>();
            var resultResult = (ObjectResult)result.Result;
            //resultResult.Value
            resultResult.StatusCode.Should().Equals(StatusCodes.Status500InternalServerError);
            resultResult.Value.Should().BeEquivalentTo("Velocity information is missing or v<=0");
        }


        private Ship CreateShip(long id)
        {
            Random rnd = new Random();
            return new Ship
            {
                Id = id,
                Name = $"Ship{id}",
                Lat = (decimal)rnd.Next(-9000, 9000) / 100,
                Long = (decimal)rnd.Next(-18000, 18000) / 100
            };
        }

        private Port CreatePort(long id)
        {
            Random rnd = new Random();
            return new Port
            {
                Id = id,
                Name = $"Ship{id}",
                Lat = (decimal)rnd.Next(-9000, 9000) / 100,
                Long = (decimal)rnd.Next(-18000, 18000) / 100
            };
        }
    }
}
