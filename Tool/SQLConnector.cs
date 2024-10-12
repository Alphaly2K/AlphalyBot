using MySqlConnector;

namespace AlphalyBot.Tool;

internal class SQLConnector
{
    public static string ConnectString;
    private static string Collation;

    private readonly MySqlConnection msc;

    public SQLConnector(string connectString, string collation)
    {
        ConnectString = connectString;
        Collation = collation;
        msc = new MySqlConnection(ConnectString);
    }

    public SQLConnector()
    {
        msc = new MySqlConnection(ConnectString);
    }

    public async Task<bool> IsGroupIdExist(long groupId)
    {
        try
        {
            await msc.OpenAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine("数据库无法连接");
#if DEBUG
            Console.WriteLine(ex.ToString());
#endif
            Environment.Exit(-1);
        }

        var sql =
            $"SELECT CASE WHEN EXISTS (SELECT 1 FROM groupservices WHERE Id = {groupId}) THEN 1 ELSE 0 END AS exists_flag;";
        MySqlCommand cmd = new(sql, msc);
        var reader = await cmd.ExecuteReaderAsync();
        _ = reader.Read();
        if ((int)reader[0] == 1)
        {
            await msc.CloseAsync();
            return true;
        }

        await msc.CloseAsync();
        return false;
    }

    public async Task<string> QueryByGroupId(long groupId)
    {
        try
        {
            await msc.OpenAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine("数据库无法连接");
#if DEBUG
            Console.WriteLine(ex.ToString());
#endif
            Environment.Exit(-1);
        }

        var sql = $"select Service from groupservices where Id = {groupId}";
        MySqlCommand cmd = new(sql, msc);
        var reader = await cmd.ExecuteReaderAsync();
        _ = reader.Read();
        var tmp = (string)reader[0];
        await msc.CloseAsync();
        return tmp;
    }

    public async Task InsertByGroupId(long groupId, string services)
    {
        try
        {
            await msc.OpenAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine("数据库无法连接");
#if DEBUG
            Console.WriteLine(ex.ToString());
#endif
            Environment.Exit(-1);
        }

        var sql = $"insert into groupservices values ({groupId},{services})";
        MySqlCommand cmd = new(sql, msc);
        _ = await cmd.ExecuteNonQueryAsync();
        await msc.CloseAsync();
    }

    public async Task ChangeByGroupId(long groupId, string services)
    {
        try
        {
            await msc.OpenAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine("数据库无法连接");
#if DEBUG
            Console.WriteLine(ex.ToString());
#endif
            Environment.Exit(-1);
        }

        var sql = @$"update groupservices set Service='{services}' where Id = '{groupId}'";
        MySqlCommand cmd = new(sql, msc);
        _ = await cmd.ExecuteNonQueryAsync();
        await msc.CloseAsync();
    }

    public async Task Init()
    {
        try
        {
            await msc.OpenAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine("数据库无法连接");
#if DEBUG
            Console.WriteLine(ex.ToString());
#endif
            Environment.Exit(-1);
        }

        var sql = @"SELECT COUNT(*) FROM information_schema.tables WHERE table_name = 'groupservices';";
        MySqlCommand cmd = new(sql, msc);
        var reader = await cmd.ExecuteReaderAsync();
        _ = reader.Read();
        if (reader[0].ToString() == "0")
        {
            var createsql =
                $@"CREATE TABLE `groupservices` ( `Id` BIGINT(20) UNSIGNED ZEROFILL NOT NULL, `Service` TINYTEXT NULL DEFAULT NULL COLLATE '{Collation}', PRIMARY KEY (`Id`) USING BTREE)";
            MySqlCommand createcmd = new(createsql, msc);
            _ = await cmd.ExecuteNonQueryAsync();
        }

        await msc.CloseAsync();
    }
}