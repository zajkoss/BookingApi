using AutoMapper;
using BookingApi.DTOs;
using BookingApi.Mappings;
using BookingApi.Models;
using BookingApi.Repository;
using BookingApi.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace BookingApi.Tests.Services;

public class ResourceServiceTests
{
    public const string CacheKeyAll = "resources:all";
    private readonly Mock<IResourceRepository> _repositoryMock;
    // private readonly Mock<ICacheService> _cacheMock;
    private readonly Mock<ILogger<ResourceService>> _loggerMock;
    private readonly IResourceService _sut;
    private readonly IMapper _mapper;

    public ResourceServiceTests()
    {
        _repositoryMock = new Mock<IResourceRepository>();
        // _cacheMock = new Mock<ICacheService>();
        _loggerMock = new Mock<ILogger<ResourceService>>();

        _mapper = new MapperConfiguration(
            cfg => cfg.AddProfile<ResourceProfile>(),
            NullLoggerFactory.Instance
        ).CreateMapper();

        _sut = new ResourceService(
            _repositoryMock.Object,
            _loggerMock.Object,
            _mapper
        );
    }

    [Fact]
    public async Task GetAll_WhenCachedHit_ShouldReturnAllCachedResources()
    {
        var cachedData = new List<ResourceDto>
        {
            new ResourceDto(Guid.NewGuid(), "Room 1", null, 10, true),
        };
    
        _cacheMock.Setup(x => x.GetAsync<List<ResourceDto>>(CacheKeyAll)).ReturnsAsync(cachedData);
    
        var result = await _sut.GetAllAsync();
    
        result.Should().BeEquivalentTo(cachedData);
        _repositoryMock.Verify(x => x.GetAllAsync(), Times.Never);
    }

    // [Fact]
    // public async Task GetAll_WhenCachedMiss_ShouldReturnAllRepoResources()
    // {
    //     var repoData = new List<Resource>
    //     {
    //         new Resource()
    //         {
    //             Id = Guid.NewGuid(),
    //             Capacity = 10,
    //             Description = "Room 1",
    //             Name = "Room 1",
    //             IsActive = true,
    //             Version = 0
    //         },
    //     };
    //
    //     _cacheMock.Setup(x => x.GetAsync<List<ResourceDto>>(CacheKeyAll)).ReturnsAsync((List<ResourceDto>?)null);
    //     _repositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(repoData);
    //
    //     var result = await _sut.GetAllAsync();
    //
    //     var expectedDtos = repoData.Select(r => _mapper.Map<ResourceDto>(r));
    //     result.Should().BeEquivalentTo(expectedDtos);
    //
    //     _repositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    //     _cacheMock.Verify(x => x.SetAsync(CacheKeyAll, It.IsAny<List<ResourceDto>>(), null), Times.Once);
    // }

    [Fact]
    public async Task GetAll_WhenCachedMissAndRepoMiss_ShouldReturnEmptyList()
    {
        _cacheMock.Setup(x => x.GetAsync<List<ResourceDto>>(CacheKeyAll)).ReturnsAsync((List<ResourceDto>?)null);
        _repositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Resource>());

        var result = await _sut.GetAllAsync();

        result.Should().BeEquivalentTo(new List<ResourceDto>());
        _repositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetById_WhenCachedMissAndRepoMiss_ShouldReturnNull()
    {
        var guid = Guid.NewGuid();
        _cacheMock.Setup(x => x.GetAsync<ResourceDto>($"resources:{guid}")).ReturnsAsync((ResourceDto)null);
        _repositoryMock.Setup(x => x.GetByIdAsync(guid)).ReturnsAsync((Resource?)null);

        var result = await _sut.GetByIdAsync(guid);

        result.Should().BeNull();
        _repositoryMock.Verify(x => x.GetByIdAsync(guid), Times.Once);
        _cacheMock.Verify(x => x.GetAsync<ResourceDto>($"resources:{guid}"), Times.Once);
    }

    [Fact]
    public async Task GetById_WhenCachedHit_ShouldReturnResource()
    {
        var guid = Guid.NewGuid();
        var repoData = new ResourceDto(guid, "Room 1", null, 10, true);

        _cacheMock.Setup(x => x.GetAsync<ResourceDto>($"resources:{guid}")).ReturnsAsync(repoData);

        var result = await _sut.GetByIdAsync(guid);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(repoData);
        ;
        _cacheMock.Verify(x => x.GetAsync<ResourceDto>($"resources:{guid}"), Times.Once);
    }

    [Fact]
    public async Task GetById_WhenCachedMissAndRepoHit_ShouldReturnResource()
    {
        var guid = Guid.NewGuid();
        var repoData = new Resource()
        {
            Id = guid,
            Capacity = 10,
            Description = "Room 1",
            Name = "Room 1",
            IsActive = true,
        };

        _cacheMock.Setup(x => x.GetAsync<ResourceDto>($"resources:{guid}")).ReturnsAsync((ResourceDto)null);
        _repositoryMock.Setup(x => x.GetByIdAsync(guid)).ReturnsAsync(repoData);

        var result = await _sut.GetByIdAsync(guid);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(_mapper.Map<ResourceDto>(repoData));
        _repositoryMock.Verify(x => x.GetByIdAsync(guid), Times.Once);
        _cacheMock.Verify(x => x.GetAsync<ResourceDto>($"resources:{guid}"), Times.Once);
        _cacheMock.Verify(x => x.SetAsync($"resources:{guid}", It.IsAny<ResourceDto>(), null), Times.Once);
    }

    [Fact]
    public async Task GetById_WhenIdNull_ThrowException()
    {
        var guid = Guid.Empty;

        var act = async () => await _sut.GetByIdAsync(Guid.Empty);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Id cannot be empty");
    }
    //
    // [Fact]
    // public async Task DeactiveAsync_WhenCachedMissAndRepoHit_ShouldReturnResource()
    // {
    //     var guid = Guid.NewGuid();
    //     var repoData = new Resource()
    //     {
    //         Id = guid,
    //         Capacity = 10,
    //         Description = "Room 1",
    //         Name = "Room 1",
    //         IsActive = true,
    //     };
    //
    //     _cacheMock.Setup(x => x.GetAsync<ResourceDto>($"resources:{guid}")).ReturnsAsync((ResourceDto)null);
    //     _repositoryMock.Setup(x => x.GetByIdAsync(guid)).ReturnsAsync(repoData);
    //
    //     var result = await _sut.DeactiveAsync(guid);
    //
    //     result.Should().NotBeNull();
    //     result.Should().BeEquivalentTo(_mapper.Map<ResourceDto>(repoData));
    //     _repositoryMock.Verify(x => x.GetByIdAsync(guid), Times.Once);
    //     _cacheMock.Verify(x => x.GetAsync<ResourceDto>($"resources:{guid}"), Times.Once);
    //     _cacheMock.Verify(x => x.SetAsync($"resources:{guid}", It.IsAny<ResourceDto>(), null), Times.Once);
    // }


    /*
       dezaykente dezaktynego
       utworzenie poprawne
       utworzeie jesli jest blad. w danych
       utwoernize jesli null
       utworzniee i sprwdzenie cachu
       update poprawny
       ipdate nie poprawny
     */
}