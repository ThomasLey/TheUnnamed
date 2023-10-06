using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TheUnnamed.Core.Database.Filter;
using TheUnnamed.Core.Database.Repository;
using TheUnnamed.Web.Blazor.Service;

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
