using EHI_Assignment.Controllers;
using EHI_Assignment.Models;
using EHI_Assignment.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;
namespace EHI_Test_Project
{
    public class EmployeesControllerTests
    {       

        [Fact]
        public async Task GetEmployees_ReturnsListOfEmployeesWithAddresses()
        {
            // Arrange
            var employeeRepositoryMock = new Mock<IEmployeeRepository>();
            employeeRepositoryMock.Setup(repo => repo.GetAllEmployeesAsync())
                .ReturnsAsync(new List<Employee>
                {
                    new Employee
                    {
                        Id = 1,
                        FirstName = "Ramesh",
                        LastName = "Nair",
                        Email = "Ramesh@gmail.com",
                        Age = 30,
                        Address = new Address
                        {
                            Street = "Borivali",
                            City = "Mumbai",
                            State = "Maharashtra"
                        }
                    }
                });

            var controller = new EmployeesController(employeeRepositoryMock.Object);

            // Act
            var result = await controller.GetEmployees();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var employeeDtos = Assert.IsAssignableFrom<IEnumerable<EmployeeDto>>(okResult.Value);
            var employeeDto = employeeDtos.FirstOrDefault();

            Assert.Single(employeeDtos); // Assuming one employee is returned
            Assert.Equal(1, employeeDto.Id);
            Assert.Equal("Ramesh", employeeDto.FirstName);
            Assert.Equal("Nair", employeeDto.LastName);
            Assert.Equal("Ramesh@gmail.com", employeeDto.Email);
            Assert.Equal(30, employeeDto.Age);
            Assert.NotNull(employeeDto.Address);
            Assert.Equal("Borivali", employeeDto.Address.Street);
            Assert.Equal("Mumbai", employeeDto.Address.City);
            Assert.Equal("Maharashtra", employeeDto.Address.State);
        }


        [Fact]
        public async Task CreateEmployee_ValidData_ReturnsCreatedAtAction()
        {
            // Arrange
            var employeeRepositoryMock = new Mock<IEmployeeRepository>();
            var controller = new EmployeesController(employeeRepositoryMock.Object);

            var employeeDto = new EmployeeDto
            {
                FirstName = "Ramesh",
                LastName = "Nair",
                Email = "Ramesh@gmail.com",
                Age = 30
            };

            // Act
            var result = await controller.CreateEmployee(employeeDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var createdEmployeeDto = Assert.IsType<Employee>(createdAtActionResult.Value);

            Assert.Equal("GetEmployeeById", createdAtActionResult.ActionName);
            Assert.Equal(createdEmployeeDto.Id, createdEmployeeDto.Id);
            // Add more assertions based on your expected behavior
        }

        [Fact]
        public async Task CreateEmployee_FirstName_ReturnsBadRequest()
        {
            // Arrange
            var employeeRepositoryMock = new Mock<IEmployeeRepository>();
            var controller = new EmployeesController(employeeRepositoryMock.Object);

            // ModelState is not valid, simulating invalid data
            controller.ModelState.AddModelError("FirstName", "The FirstName field is required.");

            var employeeDto = new EmployeeDto
            {
                // Missing required FirstName
                LastName = "Nair",
                Email = "Ramesh@gmail.com",
                Age = 30
            };

            // Act
            var result = await controller.CreateEmployee(employeeDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateEmployee_LastName_ReturnsBadRequest()
        {
            // Arrange
            var employeeRepositoryMock = new Mock<IEmployeeRepository>();
            var controller = new EmployeesController(employeeRepositoryMock.Object);

            // ModelState is not valid, simulating invalid data
            controller.ModelState.AddModelError("FirstName", "The LastName field is required.");

            var employeeDto = new EmployeeDto
            {
                // Missing required FirstName
                FirstName = "Ramesh",
                Email = "Ramesh@gmail.com",
                Age = 30
            };

            // Act
            var result = await controller.CreateEmployee(employeeDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateEmployee_Email_ReturnsBadRequest()
        {
            // Arrange
            var employeeRepositoryMock = new Mock<IEmployeeRepository>();
            var controller = new EmployeesController(employeeRepositoryMock.Object);

            controller.ModelState.AddModelError("FirstName", "The LastName field is required.");

            var employeeDto = new EmployeeDto
            {
                // Missing required FirstName
                FirstName = "Ramesh",
                LastName = "Nair",
                Age = 30
            };

            // Act
            var result = await controller.CreateEmployee(employeeDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateEmployee_Empty_ReturnsBadRequest()
        {
            // Arrange
            var employeeRepositoryMock = new Mock<IEmployeeRepository>();
            var controller = new EmployeesController(employeeRepositoryMock.Object);

            controller.ModelState.AddModelError("FirstName", "The LastName field is required.");

            var employeeDto = new EmployeeDto();

            // Act
            var result = await controller.CreateEmployee(employeeDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateEmployee_ExistingEmployee_ReturnsNoContent()
        {
            // Arrange
            var employeeRepositoryMock = new Mock<IEmployeeRepository>();
            employeeRepositoryMock.Setup(repo => repo.GetEmployeeByIdAsync(1))
                .ReturnsAsync(new Employee { Id = 1 });

            var controller = new EmployeesController(employeeRepositoryMock.Object);
            var updatedEmployeeDto = new EmployeeDto
            {
                Id = 1,
                FirstName = "UpdatedFirstName",
                LastName = "UpdatedLastName",
                Email = "updated@gmail.com",
                Age = 25
                // Add other properties to match your update
            };

            // Act
            var result = await controller.UpdateEmployee(1, updatedEmployeeDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateEmployee_NonExistentEmployee_ReturnsNotFound()
        {
            // Arrange
            var employeeRepositoryMock = new Mock<IEmployeeRepository>();
            employeeRepositoryMock.Setup(repo => repo.GetEmployeeByIdAsync(1))
                .ReturnsAsync((Employee)null);

            var controller = new EmployeesController(employeeRepositoryMock.Object);
            var updatedEmployeeDto = new EmployeeDto
            {
                Id = 1,
                FirstName = "UpdatedFirstName",
                LastName = "UpdatedLastName",
                Email = "updated@gmail.com",
                Age = 25
            };

            // Act
            var result = await controller.UpdateEmployee(1, updatedEmployeeDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public async Task DeleteEmployee_ExistingEmployee_ReturnsNoContent()
        {
            // Arrange
            var employeeRepositoryMock = new Mock<IEmployeeRepository>();
            employeeRepositoryMock.Setup(repo => repo.GetEmployeeByIdAsync(1))
                .ReturnsAsync(new Employee { Id = 1 });

            var controller = new EmployeesController(employeeRepositoryMock.Object);

            // Act
            var result = await controller.DeleteEmployee(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteEmployee_NonExistentEmployee_ReturnsNotFound()
        {
            // Arrange
            var employeeRepositoryMock = new Mock<IEmployeeRepository>();
            employeeRepositoryMock.Setup(repo => repo.GetEmployeeByIdAsync(1))
                .ReturnsAsync((Employee)null);

            var controller = new EmployeesController(employeeRepositoryMock.Object);

            // Act
            var result = await controller.DeleteEmployee(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

    }
}