using Microsoft.AspNetCore.Components;
using TheUnnamed.Core.Database.Repository;

namespace TheUnnamed.Web.Blazor.Pages
{
    public partial class FilemapOverview
    {
        public ILogger<FilemapOverview> Logger { get; set; } = null!;

        [Inject] public IFilemapRepository Repository { get; set; } = null!;

        #region view fields

        protected IEnumerable<ReadFilemapEntity>? Filemaps;

        #endregion


        protected override async Task OnInitializedAsync()
        {
            Filemaps = await Repository.GetAllFilemaps();
        }

    }
}
