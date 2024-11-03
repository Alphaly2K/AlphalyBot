using MySqlConnector;
using Serilog;

namespace AlphalyBot.Tool;

internal class SqlConnector
{
    private static string _connectString;
    private static string _collation;

    private readonly MySqlConnection _msc;

    public async Task Initialize(HashSet<string> services)
    {
        Log.Information("DbConnector: Initializing SQL connection");
        var groupList = await QueryGroups();
        foreach (var group in groupList)
        {
            var serviceSet = await QueryServices(group);
            if(serviceSet.SetEquals(services))
                continue;
            var servicesToAdd = new HashSet<string>(services);
            servicesToAdd.ExceptWith(serviceSet);
            var servicesToRemove = new HashSet<string>(serviceSet);
            servicesToRemove.ExceptWith(services);
            foreach (var service in servicesToAdd)
            {
                await InsertService(group, service);
            }

            foreach (var service in servicesToRemove)
            {
                await RemoveService(group, service);
            }
        }
    }
    
    public SqlConnector(string connectString, string collation)
    {
        _connectString = connectString;
        _collation = collation;
        _msc = new MySqlConnection(_connectString);
    }

    public SqlConnector()
    {
        _msc = new MySqlConnection(_connectString);
    }

    public async Task<HashSet<long>> QueryGroups()
    {
        HashSet<long> groups = new HashSet<long>();
        try
        {
            await _msc.OpenAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal("DbConnector: Error while opening SQL connection");
#if DEBUG
            Log.Fatal(ex.ToString());
#endif
            Environment.Exit(-1);
        }
        var sql = $"SELECT Id FROM groupservices;";
        await using var command = new MySqlCommand(sql, _msc);
        var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            groups.Add(reader.GetInt64(0));
        }

        return groups;
    }
    
    public async Task<bool> IsGroupIdExist(long groupId)
    {
        try
        {
            await _msc.OpenAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal("DbConnector: Error while opening SQL connection");
#if DEBUG
            Log.Fatal(ex.ToString());
#endif
            Environment.Exit(-1);
        }
        var sql =
            $"SELECT CASE WHEN EXISTS (SELECT 1 FROM groupservices WHERE Id = @groupId) THEN 1 ELSE 0 END AS exists_flag;";
        await using var command = new MySqlCommand(sql, _msc);
        command.Parameters.AddWithValue("@groupId", groupId);
        var reader = await command.ExecuteReaderAsync();
        await reader.ReadAsync();
        await _msc.CloseAsync();
        return (int)reader[0] == 1;
    }
    
    public async Task<HashSet<string>> QueryServices(long groupId)
    {
        try
        {
            await _msc.OpenAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal("DbConnector: Error while opening SQL connection");
#if DEBUG
            Log.Fatal(ex.ToString());
#endif
            Environment.Exit(-1);
        }
        var sql = $"SELECT Service FROM groupservices WHERE Id = @groupId;";
        await using var command = new MySqlCommand(sql, _msc);
        command.Parameters.AddWithValue("@groupId", groupId);
        var reader = await command.ExecuteReaderAsync();
        var result = new HashSet<string>();
        while (await reader.ReadAsync())
        {
            result.Add(reader.GetString("Service"));
        }
        await _msc.CloseAsync();
        return result;
    }
    
    public async Task<bool> QueryServicePrivilege(long groupId, string service)
    {
        try
        {
            await _msc.OpenAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal("DbConnector: Error while opening SQL connection");
#if DEBUG
            Log.Fatal(ex.ToString());
#endif
            Environment.Exit(-1);
        }

        var sql = $"SELECT IsEnabled from groupservices where Id = @groupId and Service = @service";
        await using var command = new MySqlCommand(sql, _msc);
        command.Parameters.AddWithValue("@groupId", groupId);
        command.Parameters.AddWithValue("@service", service);
        var reader = await command.ExecuteReaderAsync();
        _ = await reader.ReadAsync();
        var tmp = (bool)reader[0];
        await _msc.CloseAsync();
        return tmp;
    }

    public async Task RemoveService(long groupId, string service)
    {
        try
        {
            await _msc.OpenAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal("DbConnector: Error while opening SQL connection");
#if DEBUG
            Log.Fatal(ex.ToString());
#endif
            Environment.Exit(-1);
        }
        var sql = "DELETE FROM GroupServices WHERE Id = @groupId AND Service = @service";
        await using var command = new MySqlCommand(sql, _msc);
        command.Parameters.AddWithValue("@groupId", groupId);
        command.Parameters.AddWithValue("@service", service);
        _ = await command.ExecuteNonQueryAsync();
        await _msc.CloseAsync();
    }
    
    public async Task InsertService(long groupId, string service)
    {
        try
        {
            await _msc.OpenAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal("DbConnector: Error while opening SQL connection");
#if DEBUG
            Log.Fatal(ex.ToString());
#endif
            Environment.Exit(-1);
        }

        var sql = "insert into groupservices values (@groupId,@service,true);";
        await using var command = new MySqlCommand(sql, _msc);
        command.Parameters.AddWithValue("@groupId", groupId);
        command.Parameters.AddWithValue("@service", service);
        _ = await command.ExecuteNonQueryAsync();
        await _msc.CloseAsync();
    }

    public async Task UpdateServicePrivilege(long groupId, string service, bool isEnabled)
    {
        try
        {
            await _msc.OpenAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal("DbConnector: Error while opening SQL connection");
#if DEBUG
            Log.Fatal(ex.ToString());
#endif
            Environment.Exit(-1);
        }
        var sql = @$"UPDATE groupservices SET IsEnabled= @isEnabled WHERE Id = @id AND Service = @service ";
        await using var command = new MySqlCommand(sql, _msc);
        command.Parameters.AddWithValue("@isEnabled", isEnabled);
        command.Parameters.AddWithValue("@id", groupId);
        command.Parameters.AddWithValue("@service", service);
        await command.ExecuteNonQueryAsync();
        await _msc.CloseAsync();
    }
    public async Task Init()
    {
        try
        {
            await _msc.OpenAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal("DbConnector: Error while opening SQL connection");
#if DEBUG
            Log.Fatal(ex.ToString());
#endif
            Environment.Exit(-1);
        }

        Log.Information("DbConnector: Initializing SQL connection");
        var sql = @"SELECT COUNT(*) FROM information_schema.tables WHERE table_name = 'groupservices';";
        MySqlCommand cmd = new(sql, _msc);
        var reader = await cmd.ExecuteReaderAsync();
        _ = reader.Read();
        if (reader[0].ToString() == "0")
        {
            Log.Information("DbConnector: Table 'groupservices' does not exist");
        }

        await _msc.CloseAsync();
    }
}