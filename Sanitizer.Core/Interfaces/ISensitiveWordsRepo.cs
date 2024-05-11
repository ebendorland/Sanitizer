using Sanitizer.Core.Models;

namespace Sanitizer.Core.Interfaces;

public interface ISensitiveWordsRepo
{

    Task<PaginationResponse<string>> GetSensitiveWords(int page, int pageSize, int sortOrder, string search = "");

    Task<IEnumerable<string>> GetSensitiveWords(string dirtyString);

    Task<string> GetSensitiveWord(string word);

    Task CreateSensitiveWord(string word);

    Task UpdateSensitiveWord(SensitiveWord word);

    Task DeleteSensitiveWord(string word);

}
