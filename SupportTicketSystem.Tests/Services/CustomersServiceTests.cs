using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using SupportTicketSystem.Infrastructure.Services;
using SupportTicketSystem.Core.Interfaces.ICustomer;
using SupportTicketSystem.Core.Interfaces;
using SupportTicketSystem.Core.Dtos;
using SupportTicketSystem.Core.Entity;
using SupportTicketSystem.Core.Models;
using SupportTicketSystem.Core.Exceptions;

namespace SupportTicketSystem.Tests.Services
{
    public class CustomersServiceTests
    {
        private readonly Mock<IEfCoreCustomersRepo> repoMock;
        private readonly Mock<IHelperRepo> helperMock;
        private readonly IConfiguration config;
        private readonly CustomersService service;

        public CustomersServiceTests()
        {
            repoMock = new Mock<IEfCoreCustomersRepo>();
            helperMock = new Mock<IHelperRepo>();
            config = BuildConfiguration();
            service = new CustomersService(repoMock.Object, helperMock.Object, config);
        }

        private IConfiguration BuildConfiguration()
        {
            var mock = new Mock<IConfiguration>();
            mock.SetupGet(c => c["Pagination:DefaultPageSize"]).Returns("10");
            mock.SetupGet(c => c["Pagination:MaxPageSize"]).Returns("100");
            return mock.Object;
        }

        [Fact]
        public async Task getCustomerById_notFound_throwsAppException404()
        {
            repoMock.Setup(r =>
                r.getCustomerByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Customer?)null);

            AppException ex = await Assert.ThrowsAsync<AppException>(() => service.getCustomerByIdAsync(5));
            Assert.Equal(404, ex.status);
        }

        [Fact]
        public async Task createCustomer_duplicate_throwsAppException409()
        {
            helperMock.Setup(h => h.doesCustomerDetailsExistsAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

            CreateCustomerDto dto = new CreateCustomerDto { name = "n", email = "e@x.com", phone = "123" };

            AppException ex = await Assert.ThrowsAsync<AppException>(() => service.createCustomerAsync(dto));
            Assert.Equal(409, ex.status);
        }

        [Fact]
        public async Task createCustomer_success_returnsResponse()
        {
            helperMock.Setup(h => h.doesCustomerDetailsExistsAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);

            Customer created = new Customer { id = 7, name = "Name", email = "a@b.com", phone = "555" };
            repoMock.Setup(r => r.createCustomerAsync(It.IsAny<Customer>())).ReturnsAsync(created);

            CreateCustomerDto dto = new CreateCustomerDto { name = "Name", email = "a@b.com", phone = "555" };

            ResponseCustomerDto res = await service.createCustomerAsync(dto);

            Assert.Equal(7, res.id);
            Assert.Equal("Name", res.name);
        }

        [Fact]
        public async Task deleteCustomerWithNoOpenTickets_notExists_throws404()
        {
            helperMock.Setup(h => h.doesCustomerExistsAsync(It.IsAny<int>())).ReturnsAsync(false);

            AppException ex = await Assert.ThrowsAsync<AppException>(() => service.deleteCustomerWithNoOpenTicketsAsync(It.IsAny<int>()));
            Assert.Equal(404, ex.status);
        }

        [Fact]
        public async Task deleteCustomerWithNoOpenTickets_hasOpenTickets_throws400()
        {
            helperMock.Setup(h => h.doesCustomerExistsAsync(It.IsAny<int>())).ReturnsAsync(true);
            repoMock.Setup(r => r.deleteCustomerWithNoOpenTicketsAsync(It.IsAny<int>())).ReturnsAsync(false);

            AppException ex = await Assert.ThrowsAsync<AppException>(() => service.deleteCustomerWithNoOpenTicketsAsync(3));
            Assert.Equal(400, ex.status);
        }

        [Fact]
        public async Task deleteCustomerWithNoOpenTickets_success_returnsTrue()
        {
            helperMock.Setup(h => h.doesCustomerExistsAsync(It.IsAny<int>())).ReturnsAsync(true);
            repoMock.Setup(r => r.deleteCustomerWithNoOpenTicketsAsync(It.IsAny<int>())).ReturnsAsync(true);

            bool res = await service.deleteCustomerWithNoOpenTicketsAsync(3);

            Assert.True(res);
        }
    }
}
