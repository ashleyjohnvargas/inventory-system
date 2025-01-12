using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using InventorySystem.Controllers;
using InventorySystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace InventorySystemTests
{
	[TestFixture]
	public class DashboardControllerTests
	{
		private Mock<DbSet<Product>> _mockProductSet;
		private ApplicationDbContext _mockContext;
		private DashboardController _controller;
		private DbContextOptions<ApplicationDbContext> options;

		[SetUp]
		public void SetUp()
		{
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
			   .UseInMemoryDatabase(databaseName: "TestDatabase")
			   .Options;

			_mockContext = new ApplicationDbContext(options);
			_controller = new DashboardController(_mockContext);

			// Sample data for testing
			var products = new List<Product>
			{
				new Product { Id = 1, Name = "Product A", CurrentStock = 10, Price = 100, IsDeleted = false, Category = "Category 1", DateAdded = DateTime.Today },
				new Product { Id = 2, Name = "Product B", CurrentStock = 0, Price = 200, IsDeleted = false, Category = "Category 2", DateAdded = DateTime.Today.AddDays(-1) },
				new Product { Id = 3, Name = "Product C", CurrentStock = 5, Price = 150, IsDeleted = true, Category = "Category 1", DateAdded = DateTime.Today.AddDays(-7) },
				new Product { Id = 4, Name = "Product D", CurrentStock = 20, Price = 300, IsDeleted = false, Category = "Category 2", DateAdded = DateTime.Today.AddMonths(-1) },
				new Product { Id = 5, Name = "Product E", CurrentStock = 15, Price = 250, IsDeleted = false, Category = "Category 3", DateAdded = DateTime.Today.AddMonths(-2) },
			}.AsQueryable();

			// Mock DbSet
			_mockProductSet = new Mock<DbSet<Product>>();
			_mockProductSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(products.Provider);
			_mockProductSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(products.Expression);
			_mockProductSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(products.ElementType);
			_mockProductSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(products.GetEnumerator());

			// Mock DbContext
			_mockContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
			_mockContext.Setup(c => c.Products).Returns(_mockProductSet.Object);

			// Initialize controller
			_controller = new DashboardController(_mockContext.Object);
		}

		[Test]
		public void Index_ReturnsCorrectViewModel()
		{
			// Act
			var result = _controller.Index() as ViewResult;
			var model = result?.Model as DashboardViewModel;

			// Assert
			Assert.IsNotNull(model);
			Assert.AreEqual(4, model.TotalProducts); // Only non-deleted products counted
			Assert.AreEqual(1, model.OutOfStockProducts); // Non-deleted and out-of-stock products

			// Verify top products by quantity
			Assert.AreEqual(3, model.TopProductsByQuantity.Count);
			Assert.AreEqual("Product D", model.TopProductsByQuantity.First().Name);

			// Verify top products by price
			Assert.AreEqual(3, model.TopProductsByPrice.Count);
			Assert.AreEqual("Product D", model.TopProductsByPrice.First().Name);

			// Verify top categories by stock
			Assert.AreEqual(3, model.TopCategoriesByStock.Count);
			Assert.AreEqual("Category 2", model.TopCategoriesByStock.First().CategoryName);

			// Verify products added time frames
			Assert.AreEqual(1, model.ProductsAddedToday);
			Assert.AreEqual(2, model.ProductsAddedThisWeek);
			Assert.AreEqual(2, model.ProductsAddedThisMonth);
			Assert.AreEqual(4, model.ProductsAddedThisYear);
		}

		[Test]
		public void Index_HandlesEmptyDatabaseCorrectly()
		{
			// Arrange
			_mockProductSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(new List<Product>().GetEnumerator());

			// Act
			var result = _controller.Index() as ViewResult;
			var model = result?.Model as DashboardViewModel;

			// Assert
			Assert.IsNotNull(model);
			Assert.AreEqual(0, model.TotalProducts);
			Assert.AreEqual(0, model.OutOfStockProducts);
			Assert.IsEmpty(model.TopProductsByQuantity);
			Assert.IsEmpty(model.TopProductsByPrice);
			Assert.IsEmpty(model.TopCategoriesByStock);
			Assert.AreEqual(0, model.ProductsAddedToday);
			Assert.AreEqual(0, model.ProductsAddedThisWeek);
			Assert.AreEqual(0, model.ProductsAddedThisMonth);
			Assert.AreEqual(0, model.ProductsAddedThisYear);
		}


		[TearDown]
		public void TearDown()
		{
			_controller.Dispose();
			_mockContext.Database.EnsureDeleted();
			_mockContext.Dispose();
		}

	}
}
