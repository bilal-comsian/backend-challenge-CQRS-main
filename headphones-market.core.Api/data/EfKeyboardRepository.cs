using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using headphones_market.core.Api.Model;

namespace headphones_market.core.Api.Data;

public class EfKeyboardRepository : IKeyboardRepository
{
    private readonly HeadphonesDbContext _db;

    public EfKeyboardRepository(HeadphonesDbContext db)
    {
        _db = db;
    }

    public async Task<List<Keyboard>> GetAllAsync()
    {
        var results = new List<Keyboard>();
        var sql = @"
SELECT 
  p.Id, p.Name, p.Description, p.Price, p.ImageFileName, p.Wireless, p.Weight,
  k.IsMechanical
FROM dbo.Products p
INNER JOIN dbo.Keyboards k ON p.Id = k.Id;";

        var conn = _db.Database.GetDbConnection();
        await using (conn)
        {
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(ReadKeyboard(reader));
            }
        }

        return results;
    }

    public async Task<Keyboard?> GetByIdAsync(int id)
    {
        var sql = @"
SELECT 
  p.Id, p.Name, p.Description, p.Price, p.ImageFileName, p.Wireless, p.Weight,
  k.IsMechanical
FROM dbo.Products p
INNER JOIN dbo.Keyboards k ON p.Id = k.Id
WHERE p.Id = @Id;";

        var conn = _db.Database.GetDbConnection();
        await using (conn)
        {
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            var p = cmd.CreateParameter();
            p.ParameterName = "@Id";
            p.DbType = DbType.Int32;
            p.Value = id;
            cmd.Parameters.Add(p);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return ReadKeyboard(reader);
        }

        return null;
    }

    public async Task<Keyboard> AddAsync(Keyboard item)
    {
        // ensure DB generates identity
        item.Id = 0;

        var conn = _db.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync();

        await using var transaction = await conn.BeginTransactionAsync();
        try
        {
            var insertProductSql = @"
INSERT INTO dbo.Products (Name, Description, Price, ImageFileName, Wireless, Weight)
OUTPUT INSERTED.Id
VALUES (@Name, @Description, @Price, @ImageFileName, @Wireless, @Weight);";

            await using var prodCmd = conn.CreateCommand();
            prodCmd.Transaction = transaction;
            prodCmd.CommandText = insertProductSql;

            var pName = prodCmd.CreateParameter(); pName.ParameterName = "@Name"; pName.DbType = DbType.String; pName.Value = (object?)item.Name ?? DBNull.Value; prodCmd.Parameters.Add(pName);
            var pDesc = prodCmd.CreateParameter(); pDesc.ParameterName = "@Description"; pDesc.DbType = DbType.String; pDesc.Value = (object?)item.Description ?? DBNull.Value; prodCmd.Parameters.Add(pDesc);
            var pPrice = prodCmd.CreateParameter(); pPrice.ParameterName = "@Price"; pPrice.DbType = DbType.Decimal; pPrice.Value = Convert.ToDecimal(item.Price); prodCmd.Parameters.Add(pPrice);
            var pImage = prodCmd.CreateParameter(); pImage.ParameterName = "@ImageFileName"; pImage.DbType = DbType.String; pImage.Value = (object?)item.ImageFileName ?? DBNull.Value; prodCmd.Parameters.Add(pImage);
            var pWireless = prodCmd.CreateParameter(); pWireless.ParameterName = "@Wireless"; pWireless.DbType = DbType.Boolean; pWireless.Value = item.Wireless; prodCmd.Parameters.Add(pWireless);
            var pWeight = prodCmd.CreateParameter(); pWeight.ParameterName = "@Weight"; pWeight.DbType = DbType.String; pWeight.Value = (object?)item.Weight ?? DBNull.Value; prodCmd.Parameters.Add(pWeight);

            var insertedIdObj = await prodCmd.ExecuteScalarAsync();
            var newId = Convert.ToInt32(insertedIdObj);

            var insertKeySql = @"
INSERT INTO dbo.Keyboards (Id, IsMechanical)
VALUES (@Id, @IsMechanical);";

            await using var keyCmd = conn.CreateCommand();
            keyCmd.Transaction = transaction;
            keyCmd.CommandText = insertKeySql;

            var kId = keyCmd.CreateParameter(); kId.ParameterName = "@Id"; kId.DbType = DbType.Int32; kId.Value = newId; keyCmd.Parameters.Add(kId);
            var kMech = keyCmd.CreateParameter(); kMech.ParameterName = "@IsMechanical"; kMech.DbType = DbType.Boolean; kMech.Value = item.IsMechanical; keyCmd.Parameters.Add(kMech);

            await keyCmd.ExecuteNonQueryAsync();

            await transaction.CommitAsync();

            item.Id = newId;
            return item;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
        finally
        {
            await conn.CloseAsync();
        }
    }

    public async Task<bool> UpdateAsync(Keyboard item)
    {
        var conn = _db.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync();

        await using var transaction = await conn.BeginTransactionAsync();
        try
        {
            var updateProductSql = @"
UPDATE dbo.Products
SET Name = @Name,
    Description = @Description,
    Price = @Price,
    ImageFileName = @ImageFileName,
    Wireless = @Wireless,
    Weight = @Weight
WHERE Id = @Id;";

            await using var prodCmd = conn.CreateCommand();
            prodCmd.Transaction = transaction;
            prodCmd.CommandText = updateProductSql;

            var pId = prodCmd.CreateParameter(); pId.ParameterName = "@Id"; pId.DbType = DbType.Int32; pId.Value = item.Id; prodCmd.Parameters.Add(pId);
            var pName = prodCmd.CreateParameter(); pName.ParameterName = "@Name"; pName.DbType = DbType.String; pName.Value = (object?)item.Name ?? DBNull.Value; prodCmd.Parameters.Add(pName);
            var pDesc = prodCmd.CreateParameter(); pDesc.ParameterName = "@Description"; pDesc.DbType = DbType.String; pDesc.Value = (object?)item.Description ?? DBNull.Value; prodCmd.Parameters.Add(pDesc);
            var pPrice = prodCmd.CreateParameter(); pPrice.ParameterName = "@Price"; pPrice.DbType = DbType.Decimal; pPrice.Value = Convert.ToDecimal(item.Price); prodCmd.Parameters.Add(pPrice);
            var pImage = prodCmd.CreateParameter(); pImage.ParameterName = "@ImageFileName"; pImage.DbType = DbType.String; pImage.Value = (object?)item.ImageFileName ?? DBNull.Value; prodCmd.Parameters.Add(pImage);
            var pWireless = prodCmd.CreateParameter(); pWireless.ParameterName = "@Wireless"; pWireless.DbType = DbType.Boolean; pWireless.Value = item.Wireless; prodCmd.Parameters.Add(pWireless);
            var pWeight = prodCmd.CreateParameter(); pWeight.ParameterName = "@Weight"; pWeight.DbType = DbType.String; pWeight.Value = (object?)item.Weight ?? DBNull.Value; prodCmd.Parameters.Add(pWeight);

            var productAffected = await prodCmd.ExecuteNonQueryAsync();
            if (productAffected == 0)
            {
                await transaction.RollbackAsync();
                return false;
            }

            var updateKeySql = @"
UPDATE dbo.Keyboards
SET IsMechanical = @IsMechanical
WHERE Id = @Id;";

            await using var keyCmd = conn.CreateCommand();
            keyCmd.Transaction = transaction;
            keyCmd.CommandText = updateKeySql;

            var kId = keyCmd.CreateParameter(); kId.ParameterName = "@Id"; kId.DbType = DbType.Int32; kId.Value = item.Id; keyCmd.Parameters.Add(kId);
            var kMech = keyCmd.CreateParameter(); kMech.ParameterName = "@IsMechanical"; kMech.DbType = DbType.Boolean; kMech.Value = item.IsMechanical; keyCmd.Parameters.Add(kMech);

            var keyAffected = await keyCmd.ExecuteNonQueryAsync();
            if (keyAffected == 0)
            {
                await transaction.RollbackAsync();
                return false;
            }

            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
        finally
        {
            await conn.CloseAsync();
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var conn = _db.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync();

        await using var transaction = await conn.BeginTransactionAsync();
        try
        {
            var deleteKeySql = "DELETE FROM dbo.Keyboards WHERE Id = @Id;";
            await using var keyCmd = conn.CreateCommand();
            keyCmd.Transaction = transaction;
            keyCmd.CommandText = deleteKeySql;
            var kId = keyCmd.CreateParameter(); kId.ParameterName = "@Id"; kId.DbType = DbType.Int32; kId.Value = id; keyCmd.Parameters.Add(kId);
            await keyCmd.ExecuteNonQueryAsync();

            var deleteProdSql = "DELETE FROM dbo.Products WHERE Id = @Id;";
            await using var prodCmd = conn.CreateCommand();
            prodCmd.Transaction = transaction;
            prodCmd.CommandText = deleteProdSql;
            var pId = prodCmd.CreateParameter(); pId.ParameterName = "@Id"; pId.DbType = DbType.Int32; pId.Value = id; prodCmd.Parameters.Add(pId);
            var affected = await prodCmd.ExecuteNonQueryAsync();

            await transaction.CommitAsync();
            return affected > 0;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
        finally
        {
            await conn.CloseAsync();
        }
    }

    private static Keyboard ReadKeyboard(DbDataReader reader)
    {
        return new Keyboard
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? string.Empty : reader.GetString(reader.GetOrdinal("Name")),
            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? string.Empty : reader.GetString(reader.GetOrdinal("Description")),
            Price = reader.IsDBNull(reader.GetOrdinal("Price")) ? 0.0 : Convert.ToDouble(reader.GetDecimal(reader.GetOrdinal("Price"))),
            ImageFileName = reader.IsDBNull(reader.GetOrdinal("ImageFileName")) ? string.Empty : reader.GetString(reader.GetOrdinal("ImageFileName")),
            Wireless = !reader.IsDBNull(reader.GetOrdinal("Wireless")) && reader.GetBoolean(reader.GetOrdinal("Wireless")),
            Weight = reader.IsDBNull(reader.GetOrdinal("Weight")) ? string.Empty : reader.GetString(reader.GetOrdinal("Weight")),
            IsMechanical = !reader.IsDBNull(reader.GetOrdinal("IsMechanical")) && reader.GetBoolean(reader.GetOrdinal("IsMechanical"))
        };
    }
}