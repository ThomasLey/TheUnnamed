using Microsoft.AspNetCore.Components.Authorization;
using TheUnnamed.Core.Database.Filter;
using TheUnnamed.Core.Database.Repository;

namespace TheUnnamed.Web.Blazor.Service;

public class ListMyDocumentsService
{
    private readonly AuthenticationStateProvider _authProvider;
    private readonly IDocumentRepository _repository;

    public ListMyDocumentsService(AuthenticationStateProvider authProvider, IDocumentRepository repository)
    {
        _authProvider = authProvider;
        _repository = repository;
    }

    public async Task<ListMyDocumentsViewModel?> GetDocuments()
    {
        //  we need to convert the ReadDocumentEntity to a ViewModel
        return (await _repository.GetDocumentsAsync(new SortAndFilter())).ToViewModel();
    }
}

public class ListMyDocumentsViewModel
{
    public IEnumerable<DocumentViewModel> Documents { get; set; }
}

public class DocumentViewModel
{
    public string? Title { get; set; }
    public string? Filename { get; set; }
    public FilemapViewModel? Filemap { get; set; }
    public string? Hash { get; set; }
    public Guid Uuid { get; set; }
}

public class FilemapViewModel
{
    public string? Title { get; set; }
    public Guid Uuid { get; set; }
}

public static class ListInboxViewModelExtensions
{
    public static ListMyDocumentsViewModel ToViewModel(this IEnumerable<ReadDocumentEntity> source)
    {
        var result = new ListMyDocumentsViewModel
        {
            Documents = source.Select(x => x.ToViewModel())
        };

        return result;
    }

    public static DocumentViewModel ToViewModel(this ReadDocumentEntity d)
    {
        return new DocumentViewModel()
        {
            Filemap = new FilemapViewModel() { Title = d.Filemap.Title, Uuid = d.Filemap.Uuid },
            Hash = d.Hash,
            Title = d.Title,
            Filename = d.Filename,
            Uuid = d.Uuid
        };
    }
}