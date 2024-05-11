using Sanitizer.Core.Interfaces;
using System.Text.RegularExpressions;

namespace Sanitizer.Library.Services;
public class SanitizerService
{
    private readonly ISensitiveWordsRepo _repo;

    public SanitizerService(ISensitiveWordsRepo repo)
    {
        _repo = repo;
    }

    public async Task<string> SanitizeString(string dirtyString)
    {
        var sensitiveWords = await _repo.GetSensitiveWords(dirtyString);

        sensitiveWords = [.. sensitiveWords.OrderByDescending(x => x.Length)];

        dirtyString = sensitiveWords.Aggregate(dirtyString, (current, word) =>
            current.Replace(word, new string('*', word.Length), StringComparison.OrdinalIgnoreCase));

        return dirtyString;
    }
}
