using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using SupportTicketSystem.Infrastructure.Services;
using SupportTicketSystem.Core.Interfaces.ITicket;
using SupportTicketSystem.Core.Interfaces;
using SupportTicketSystem.Core.Dtos;
using SupportTicketSystem.Core.Entity;
using SupportTicketSystem.Core.Enums;
using SupportTicketSystem.Core.Models;
using SupportTicketSystem.Core.Exceptions;

namespace SupportTicketSystem.Tests.Services
{
    public class TicketsServiceTests
    {
        private readonly Mock<IEfCoreTicketsRepo> repoMock;
        private readonly Mock<IAdoRepo> adoMock;
        private readonly Mock<IHelperRepo> helperMock;
        private readonly IConfiguration config;
        private readonly TicketsService service;

        public TicketsServiceTests()
        {
            repoMock = new Mock<IEfCoreTicketsRepo>();
            adoMock = new Mock<IAdoRepo>();
            helperMock = new Mock<IHelperRepo>();
            config = BuildConfiguration();
            service = new TicketsService(repoMock.Object, adoMock.Object, helperMock.Object, config);
        }

        private IConfiguration BuildConfiguration()
        {
            var mock = new Mock<IConfiguration>();
            mock.SetupGet(c => c["Pagination:DefaultPageSize"]).Returns("10");
            mock.SetupGet(c => c["Pagination:MaxPageSize"]).Returns("100");
            return mock.Object;
        }

        [Fact]
        public async Task createTicket_invalidCustomer_throwsAppException()
        {
            helperMock.Setup(h => h.doesCustomerExistsAsync(It.IsAny<int>())).ReturnsAsync(false);

            CreateTicketDto dto = new CreateTicketDto { title = "t", description = "d", priority = PriorityValues.low, customerId = 1 };

            AppException ex = await Assert.ThrowsAsync<AppException>(() => service.createTicketAsync(dto));
            Assert.Equal(400, ex.status);
        }

        [Fact]
        public async Task createTicket_validCustomer_returnsResponse()
        {
            helperMock.Setup(h => h.doesCustomerExistsAsync(It.IsAny<int>())).ReturnsAsync(true);

            Ticket created = new Ticket { id = 5, title = "t", description = "d", priority = PriorityValues.medium, status = StatusValues.open, customerId = 1 };
            repoMock.Setup(r => r.createTicketAsync(It.IsAny<Ticket>())).ReturnsAsync(created);

            CreateTicketDto dto = new CreateTicketDto { title = "t", description = "d", priority = PriorityValues.medium, customerId = 1 };

            ResponseTicketDto res = await service.createTicketAsync(dto);

            Assert.Equal(5, res.id);
            Assert.Equal("t", res.title);
            Assert.Equal(PriorityValues.medium, res.priority);
        }

        [Fact]
        public async Task getTicketById_notFound_returnsNull()
        {
            repoMock.Setup(r => r.getTicketByIdAsync(It.IsAny<int>())).ReturnsAsync((Ticket?)null);

            ResponseTicketDto? res = await service.getTicketByIdAsync(123);

            Assert.Null(res);
        }

        [Fact]
        public async Task updateTicketStatus_ticketNotExists_throwsAppException()
        {
            helperMock.Setup(h => h.doesTicketExistsAsync(It.IsAny<int>())).ReturnsAsync(false);

            AppException ex = await Assert.ThrowsAsync<AppException>(() => service.updateTicketStatusAsync(1, StatusValues.closed));
            Assert.Equal(400, ex.status);
        }

        [Fact]
        public async Task updateTicketStatus_alreadyClosed_throwsAppException409()
        {
            helperMock.Setup(h => h.doesTicketExistsAsync(It.IsAny<int>())).ReturnsAsync(true);
            adoMock.Setup(a => a.updateTicketStatusAsync(It.IsAny<int>(), It.IsAny<StatusValues>())).ReturnsAsync(409);

            AppException ex = await Assert.ThrowsAsync<AppException>(() => service.updateTicketStatusAsync(1, StatusValues.closed));
            Assert.Equal(409, ex.status);
        }

        [Fact]
        public async Task updateTicketStatus_success_returnsTrue()
        {
            helperMock.Setup(h => h.doesTicketExistsAsync(It.IsAny<int>())).ReturnsAsync(true);
            adoMock.Setup(a => a.updateTicketStatusAsync(It.IsAny<int>(), It.IsAny<StatusValues>())).ReturnsAsync(1);

            bool result = await service.updateTicketStatusAsync(1, StatusValues.inProgress);

            Assert.True(result);
        }
    }
}
