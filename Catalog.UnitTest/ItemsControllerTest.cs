using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.Api.Controllers;
using Catalog.Api.Dtos;
using Catalog.Api.Entities;
using Catalog.Api.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Catalog.UnitTest
{
    public class ItemsControllerTest
    {
        private readonly Mock<IItemsRepository> repositoryStub = new();
        private readonly Mock<ILogger<ItemsController>> loggerStub = new();
        private readonly Random rand = new();

        [Fact]
        public async Task GetItemAsync_WithUnexistingItem_ReturnsNotFound()
        {
            // Arange
            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Item)null);

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            //Act
            var result = await controller.GetItemAsync(Guid.NewGuid());

            //Assert
            //Assert.IsType<NotFoundResult>(result.Result);
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetItemAsync_WithExistingItem_ReturnsExpectedItem()
        {
            // Arange
            var expectedItem = CreateRandomItem();

            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedItem);

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            //Act
            var result = await controller.GetItemAsync(Guid.NewGuid());

            //Assert
            result.Value.Should().BeEquivalentTo(expectedItem);
        }

        [Fact]
        public async Task GetItemAsync_WithExistingItems_ReturnsAllItems()
        {
            // Arange
            var expectedItems = new[] { CreateRandomItem(), CreateRandomItem(), CreateRandomItem() };

            repositoryStub.Setup(repo => repo.GetItemsAsync())
                .ReturnsAsync(expectedItems);

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            //Act
            var actualItems = await controller.GetItemsAsync();

            //Assert
            actualItems.Should().BeEquivalentTo(expectedItems); 
        }

        [Fact]
        public async Task GetItemAsync_WithMatchingItems_ReturnsMatchingItems()
        {
            // Arange
            var allItems = new[] 
            {
                 new Item(){ Name= "Potion"}, 
                 new Item(){ Name= "Antidote"},
                 new Item(){ Name= "Hi-Potion"} 
            };

            var nameToMatch = "Potion";

            repositoryStub.Setup(repo => repo.GetItemsAsync())
                .ReturnsAsync(allItems);

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            //Act
            IEnumerable<ItemDto> foundItems = await controller.GetItemsAsync(nameToMatch);

            //Assert
            foundItems.Should().OnlyContain(
                item => item.Name == allItems[0].Name || item.Name == allItems[2].Name
            );
        }

        [Fact]
        public async Task CreateItemAsync_WithItemToCreate_ReturnsCreatedItem()
        {
            // Arange
            var itemToCreate = new CreateItemDto( 
                 Guid.NewGuid().ToString(),
                 Guid.NewGuid().ToString(),
                 rand.Next(1000)
            );

            // var itemToCreate = new CreateItemDto()
            // { 
            //     Name = Guid.NewGuid().ToString(),
            //     Price = rand.Next(1000)
            // };

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            //Act
            var result = await controller.CreateItemAsync(itemToCreate);

            //Assert
            var createdItem = (result.Result as CreatedAtActionResult).Value as ItemDto;
            itemToCreate.Should().BeEquivalentTo(
                createdItem,
                options => options.ComparingByMembers<ItemDto>().ExcludingMissingMembers()
            );
            createdItem.Id.Should().NotBeEmpty();
            createdItem.CreateDate.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromMinutes(1000));
        }

    [Fact]
        public async Task UpdateItemAsync_WithExistingItem_ReturnsNoContent()
        {
            // Arange
            var existingItem = CreateRandomItem();

            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingItem);

            var itemId = existingItem.Id;
            var itemToUpdate = new UpdateItemDto(
                 Guid.NewGuid().ToString(),
                 Guid.NewGuid().ToString(),
                 existingItem.Price + 3
            );

            // var itemToUpdate = new UpdateItemDto()
            // {
            //     Name = Guid.NewGuid().ToString(),
            //     Price = existingItem.Price + 3
            // };

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            //Act
            var result = await controller.UpdateItemAsync(itemId, itemToUpdate);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }




        private Item CreateRandomItem()
        {
            return new()
            {
                Id=Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                Price = rand.Next(1000),
                CreateDate = DateTimeOffset.UtcNow              
            };
        }
    }
}
