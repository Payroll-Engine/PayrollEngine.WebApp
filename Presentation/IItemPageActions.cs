using System.Threading.Tasks;

namespace PayrollEngine.WebApp.Presentation
{
    public interface IItemPageActions
    {
        /// <summary>
        /// Reset all grid filters
        /// </summary>
        Task ResetFilterAsync();

        /// <summary>
        /// Download excel file from unfiltered grid data
        /// </summary>
        Task ExcelDownloadAsync(string name);
    }
}
