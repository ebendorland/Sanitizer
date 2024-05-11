using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Sanitizer.Core.Models;

public class SensitiveWord
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public string OldWord { get; set; }
    public string NewWord { get; set; }

    public SensitiveWord(string oldWord, string newWord)
    { 
        OldWord = oldWord;
        NewWord = newWord;
    }
}
