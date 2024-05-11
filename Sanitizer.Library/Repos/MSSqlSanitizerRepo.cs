using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sanitizer.Core.Exceptions;
using Sanitizer.Core.Interfaces;
using Sanitizer.Core.Models;
using System.Data;
using System.Data.SqlClient;

namespace Sanitizer.Library.Repos;
public class MSSqlSanitizerRepo(IConfiguration configuration, ILogger<MSSqlSanitizerRepo> log) : ISensitiveWordsRepo
{
    private readonly string _connectionString = configuration.GetConnectionString("MSSql");
    private readonly ILogger<MSSqlSanitizerRepo> _log = log;

    public async Task CreateSensitiveWord(string word)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("InsertSensitiveWord", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Word", word);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        catch (SqlException ex)
        {
            throw MapApiException(ex.Number, ex.Message, ex?.InnerException);
        }
    }


    public async Task DeleteSensitiveWord(string word)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("DeleteSensitiveWord", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Word", word);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting sensitive word: {ex.Message}");
            throw;
        }
    }

    public async Task UpdateSensitiveWord(SensitiveWord word)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("UpdateSensitiveWord", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@OldWord", word.OldWord);
                    command.Parameters.AddWithValue("@NewWord", word.NewWord);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        catch (SqlException ex)
        {
            throw MapApiException(ex.Number, ex.Message, ex?.InnerException);
        }
    }



    public async Task<PaginationResponse<string>> GetSensitiveWords(int page, int pageSize, int sortOrder, string search = "")
    {
        var sensitiveWords = new PaginationResponse<string>
        { 
            TotalCount = 0,
            Data = new List<string>(),
            Page = page,
            PageSize = pageSize
        };

        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("GetSensitiveWordsPaged", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Page", page);
                    command.Parameters.AddWithValue("@PageSize", pageSize);
                    command.Parameters.AddWithValue("@SortOrder", sortOrder);
                    command.Parameters.AddWithValue("@Search", string.IsNullOrEmpty(search) ? DBNull.Value : (object)search);

                    var totalCountParameter = command.Parameters.Add("@TotalCount", SqlDbType.Int);
                    totalCountParameter.Direction = ParameterDirection.Output;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            sensitiveWords.Data.Add(reader["Word"].ToString());
                        }
                    }
                    if (totalCountParameter.Value != DBNull.Value && totalCountParameter.Value != null)
                        sensitiveWords.TotalCount = (int)totalCountParameter.Value;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving sensitive words: {ex.Message}");
            throw;
        }

        return sensitiveWords;
    }

    public async Task<IEnumerable<string>> GetSensitiveWords(string dirtyString)
    {
        var sensitiveWords = new List<string>();

        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("FindSensitiveWords", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@DirtyString", dirtyString);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            sensitiveWords.Add(reader["Word"].ToString());
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving sensitive words: {ex.Message}");
            throw;
        }

        return sensitiveWords;
    }

    public async Task<string> GetSensitiveWord(string word)
    {
        string sensitiveWord = "";

        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("GetSensitiveWord", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Word", word);

                    var result = await command.ExecuteScalarAsync();
                    if (result != null)
                    {
                        sensitiveWord = result.ToString();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new ApiException(400, "An error has occured", ex);
        }

        if (string.IsNullOrEmpty(sensitiveWord))
            throw new ApiException(404, "Word doesn't exist", null);

        return sensitiveWord;
    }

    private ApiException MapApiException(int sqlCode, string message, Exception? innerException)
    {
        return sqlCode switch
        {
            2601 => new ApiException(409, "Word already exists", innerException),
            2627 => new ApiException(409, "Word already exists", innerException),
            50000 => new ApiException(400, message, innerException),
            _ => new ApiException(400, message, innerException)
        };
    }
}

