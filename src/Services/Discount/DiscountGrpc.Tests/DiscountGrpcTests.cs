using AutoFixture;
using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Mapping;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Discount.Grpc.Services;
using Grpc.Core;
using Grpc.Core.Testing;
using Microsoft.Extensions.Logging;
using Moq;

namespace DiscountGrpc.Tests
{
    public class DiscountGrpcTests
    {
        private readonly DiscountService _discountService;
        private readonly Mock<IDiscountRepository> _mockDiscountRepo;
        private readonly Mock<ILogger<DiscountService>> _mockLogger;
        private readonly IFixture _fixture;
        public DiscountGrpcTests()
        {
            _mockDiscountRepo = new Mock<IDiscountRepository>();

            var _mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DiscountProfile());
            });

            var _mapper = _mockMapper.CreateMapper();

            _mockLogger = new Mock<ILogger<DiscountService>>();

            _discountService = new DiscountService(
                _mockDiscountRepo.Object, _mapper, _mockLogger.Object);

            _fixture = new Fixture();

        }

        [Fact]
        public async void CreateDiscountTest()
        {
            //Arrange
            var serverCallContext = TestServerCallContext.Create(
                method: nameof(_discountService.CreateDiscount)
                                           , host: "localhost"
                                           , deadline: DateTime.Now.AddMinutes(30)
                                           , requestHeaders: new Metadata()
                                           , cancellationToken: CancellationToken.None
                                           , peer: "10.0.0.25:5001"
                                           , authContext: null
                                           , contextPropagationToken: null
                                           , writeHeadersFunc: (metadata) => Task.CompletedTask
                                           , writeOptionsGetter: () => new WriteOptions()
                                           , writeOptionsSetter: (writeOptions) => { }
           );

            var coupon = _fixture.Build<CouponModel>()
                             .With(x => x.Id, 1)
                             .Create();

            var createDiscountRequest = _fixture.Build<CreateDiscountRequest>()
                                                .With(x => x.Coupon, coupon)
                                                .Create();

            _mockDiscountRepo
                .Setup(x => x.CreateDiscount(It.IsAny<Coupon>()))
                .ReturnsAsync(true);

            //Act
            var response = await _discountService.CreateDiscount(createDiscountRequest, serverCallContext);

            //Assert
            Assert.Equal(response.Id, coupon.Id);
            Assert.Equivalent(response, createDiscountRequest.Coupon);

            _mockLogger.Verify(
                    x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString() == $"Discount is successfully created. ProductName: {coupon.ProductName}"),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));

            _mockDiscountRepo
                .Verify(x => x.CreateDiscount(It.IsAny<Coupon>()), Times.Once);
        }

        [Fact]
        public async Task CreateDiscountShouldThrowException()
        {
            _mockDiscountRepo
                .Setup(x => x.CreateDiscount(It.IsAny<Coupon>()))
                .ReturnsAsync(false);

            var createDiscountRequest = _fixture.Create<CreateDiscountRequest>();

            var serverCallContext = TestServerCallContext.Create(
                 method: nameof(_discountService.CreateDiscount)
                                            , host: "localhost"
                                            , deadline: DateTime.Now.AddMinutes(30)
                                            , requestHeaders: new Metadata()
                                            , cancellationToken: CancellationToken.None
                                            , peer: "10.0.0.25:5001"
                                            , authContext: null
                                            , contextPropagationToken: null
                                            , writeHeadersFunc: (metadata) => Task.CompletedTask
                                            , writeOptionsGetter: () => new WriteOptions()
                                            , writeOptionsSetter: (writeOptions) => { }
            );

            await Assert.ThrowsAsync<RpcException>(() => _discountService.CreateDiscount(createDiscountRequest, serverCallContext));

            _mockDiscountRepo
                .Verify(x => x.CreateDiscount(It.IsAny<Coupon>()), Times.Once);

        }

        [Fact]
        public async void UpdateDiscountTest()
        {
            var couponToUpdate = _fixture.Create<CouponModel>();

            var coupon = _fixture.Build<Coupon>()
                             .With(x => x.Id, couponToUpdate.Id)
                             .With(x => x.Amount, couponToUpdate.Amount)
                             .With(x => x.ProductName, couponToUpdate.ProductName)
                             .Create();

            _mockDiscountRepo
                .Setup(x => x.GetDiscount(It.IsAny<string>()))
                .ReturnsAsync(coupon);

            _mockDiscountRepo
                .Setup(x => x.UpdateDiscount(It.IsAny<Coupon>()))
                .ReturnsAsync(true);

            var updateRequest = _fixture.Create<UpdateDiscountRequest>();

            var serverCallContext = TestServerCallContext.Create(
                 method: nameof(_discountService.UpdateDiscount)
                                            , host: "localhost"
                                            , deadline: DateTime.Now.AddMinutes(30)
                                            , requestHeaders: new Metadata()
                                            , cancellationToken: CancellationToken.None
                                            , peer: "10.0.0.25:5001"
                                            , authContext: null
                                            , contextPropagationToken: null
                                            , writeHeadersFunc: (metadata) => Task.CompletedTask
                                            , writeOptionsGetter: () => new WriteOptions()
                                            , writeOptionsSetter: (writeOptions) => { });

            var response = await _discountService.UpdateDiscount(updateRequest, serverCallContext);

            Assert.Equivalent(response, updateRequest.Coupon);

        }

        [Fact]
        public async void UpdateDiscountShouldFailFromDBError()
        {
            var couponToUpdate = _fixture.Create<CouponModel>();

            var coupon = _fixture.Build<Coupon>()
                             .With(x => x.Id, couponToUpdate.Id)
                             .With(x => x.Amount, couponToUpdate.Amount)
                             .With(x => x.ProductName, couponToUpdate.ProductName)
                             .Create();

            _mockDiscountRepo
                .Setup(x => x.GetDiscount(It.IsAny<string>()))
                .ReturnsAsync(coupon);

            _mockDiscountRepo
                .Setup(x => x.UpdateDiscount(It.IsAny<Coupon>()))
                .ReturnsAsync(false);

            var updateRequest = _fixture.Create<UpdateDiscountRequest>();

            var serverCallContext = TestServerCallContext.Create(
                 method: nameof(_discountService.UpdateDiscount)
                                            , host: "localhost"
                                            , deadline: DateTime.Now.AddMinutes(30)
                                            , requestHeaders: new Metadata()
                                            , cancellationToken: CancellationToken.None
                                            , peer: "10.0.0.25:5001"
                                            , authContext: null
                                            , contextPropagationToken: null
                                            , writeHeadersFunc: (metadata) => Task.CompletedTask
                                            , writeOptionsGetter: () => new WriteOptions()
                                            , writeOptionsSetter: (writeOptions) => { });

            await Assert.ThrowsAsync<RpcException>(() => _discountService.UpdateDiscount(updateRequest, serverCallContext));
        }

        [Fact]
        public async void DeleteDiscount()
        {
            _mockDiscountRepo
                .Setup(x => x.DeleteDiscount(It.IsAny<string>()))
                .ReturnsAsync(true);

            var serverCallContext = TestServerCallContext.Create(
                 method: nameof(_discountService.DeleteDiscount)
                                            , host: "localhost"
                                            , deadline: DateTime.Now.AddMinutes(30)
                                            , requestHeaders: new Metadata()
                                            , cancellationToken: CancellationToken.None
                                            , peer: "10.0.0.25:5001"
                                            , authContext: null
                                            , contextPropagationToken: null
                                            , writeHeadersFunc: (metadata) => Task.CompletedTask
                                            , writeOptionsGetter: () => new WriteOptions()
                                            , writeOptionsSetter: (writeOptions) => { }
            );

            var deleteDiscountRequest = _fixture.Create<DeleteDiscountRequest>();

            var response = await _discountService.DeleteDiscount(deleteDiscountRequest, serverCallContext);

            Assert.True(response.Success);

            _mockDiscountRepo
                .Verify(x => x.DeleteDiscount(It.IsAny<string>()), Times.Once);
        }
    }
}