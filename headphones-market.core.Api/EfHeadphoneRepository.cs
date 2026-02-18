using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using headphones_market.core.Api.Model;

namespace headphones_market.core.Api.Data;

public class EfHeadphoneRepository : IHeadphoneRepository
{
    private readonly HeadphonesDbContext _db;

    public EfHeadphoneRepository(HeadphonesDbContext db)
    {
        _db = db;
    }

    public async Task<List<Headphone>> GetAllAsync()
    {
        var results = new List<Headphone>();
        var sql = @"
SELECT 
  p.Id, p.Name, p.Description, p.Price, p.ImageFileName, p.Wireless, p.Weight,
  h.Manufacturer, h.Color, h.Type, h.BatteryLife, h.NoiseCancellationType, h.Mic
FROM dbo.Products p
INNER JOIN dbo.Headphones h ON p.Id = h.Id;";

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
                results.Add(ReadHeadphone(reader));
            }
        }

        return results;
    }

    public async Task<Headphone?> GetByIdAsync(int id)
    {
        var sql = @"
SELECT 
  p.Id, p.Name, p.Description, p.Price, p.ImageFileName, p.Wireless, p.Weight,
  h.Manufacturer, h.Color, h.Type, h.BatteryLife, h.NoiseCancellationType, h.Mic
FROM dbo.Products p
INNER JOIN dbo.Headphones h ON p.Id = h.Id
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
                return ReadHeadphone(reader);
        }

        return null;
    }

    public async Task<Headphone> AddAsync(Headphone item)
    {
        // Ensure DB generates identity
        item.Id = 0;

        var conn = _db.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync();

        await using var transaction = await conn.BeginTransactionAsync();

        try
        {
            // 1) Insert into Products and get new Id
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

            // 2) Insert into Headphones using newId
            var insertHeadSql = @"
INSERT INTO dbo.Headphones (Id, Manufacturer, Color, Type, BatteryLife, NoiseCancellationType, Mic)
VALUES (@Id, @Manufacturer, @Color, @Type, @BatteryLife, @NoiseCancellationType, @Mic);";

            await using var headCmd = conn.CreateCommand();
            headCmd.Transaction = transaction;
            headCmd.CommandText = insertHeadSql;

            var hId = headCmd.CreateParameter(); hId.ParameterName = "@Id"; hId.DbType = DbType.Int32; hId.Value = newId; headCmd.Parameters.Add(hId);
            var hMan = headCmd.CreateParameter(); hMan.ParameterName = "@Manufacturer"; hMan.DbType = DbType.String; hMan.Value = (object?)item.Manufacturer ?? DBNull.Value; headCmd.Parameters.Add(hMan);
            var hColor = headCmd.CreateParameter(); hColor.ParameterName = "@Color"; hColor.DbType = DbType.String; hColor.Value = (object?)item.Color ?? DBNull.Value; headCmd.Parameters.Add(hColor);
            var hType = headCmd.CreateParameter(); hType.ParameterName = "@Type"; hType.DbType = DbType.String; hType.Value = (object?)item.Type ?? DBNull.Value; headCmd.Parameters.Add(hType);
            var hBattery = headCmd.CreateParameter(); hBattery.ParameterName = "@BatteryLife"; hBattery.DbType = DbType.String; hBattery.Value = (object?)item.BatteryLife ?? DBNull.Value; headCmd.Parameters.Add(hBattery);
            var hNoise = headCmd.CreateParameter(); hNoise.ParameterName = "@NoiseCancellationType"; hNoise.DbType = DbType.String; hNoise.Value = (object?)item.NoiseCancellationType ?? DBNull.Value; headCmd.Parameters.Add(hNoise);
            var hMic = headCmd.CreateParameter(); hMic.ParameterName = "@Mic"; hMic.DbType = DbType.Boolean; hMic.Value = item.Mic; headCmd.Parameters.Add(hMic);

            await headCmd.ExecuteNonQueryAsync();

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

    public async Task<bool> UpdateAsync(Headphone item)
    {
        var conn = _db.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync();

        await using var transaction = await conn.BeginTransactionAsync();
        try
        {
            // Update Products (base fields)
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

            // Update Headphones (derived fields)
            var updateHeadSql = @"
UPDATE dbo.Headphones
SET Manufacturer = @Manufacturer,
    Color = @Color,
    Type = @Type,
    BatteryLife = @BatteryLife,
    NoiseCancellationType = @NoiseCancellationType,
    Mic = @Mic
WHERE Id = @Id;";

            await using var headCmd = conn.CreateCommand();
            headCmd.Transaction = transaction;
            headCmd.CommandText = updateHeadSql;

            var hId = headCmd.CreateParameter(); hId.ParameterName = "@Id"; hId.DbType = DbType.Int32; hId.Value = item.Id; headCmd.Parameters.Add(hId);
            var hMan = headCmd.CreateParameter(); hMan.ParameterName = "@Manufacturer"; hMan.DbType = DbType.String; hMan.Value = (object?)item.Manufacturer ?? DBNull.Value; headCmd.Parameters.Add(hMan);
            var hColor = headCmd.CreateParameter(); hColor.ParameterName = "@Color"; hColor.DbType = DbType.String; hColor.Value = (object?)item.Color ?? DBNull.Value; headCmd.Parameters.Add(hColor);
            var hType = headCmd.CreateParameter(); hType.ParameterName = "@Type"; hType.DbType = DbType.String; hType.Value = (object?)item.Type ?? DBNull.Value; headCmd.Parameters.Add(hType);
            var hBattery = headCmd.CreateParameter(); hBattery.ParameterName = "@BatteryLife"; hBattery.DbType = DbType.String; hBattery.Value = (object?)item.BatteryLife ?? DBNull.Value; headCmd.Parameters.Add(hBattery);
            var hNoise = headCmd.CreateParameter(); hNoise.ParameterName = "@NoiseCancellationType"; hNoise.DbType = DbType.String; hNoise.Value = (object?)item.NoiseCancellationType ?? DBNull.Value; headCmd.Parameters.Add(hNoise);
            var hMic = headCmd.CreateParameter(); hMic.ParameterName = "@Mic"; hMic.DbType = DbType.Boolean; hMic.Value = item.Mic; headCmd.Parameters.Add(hMic);

            var headAffected = await headCmd.ExecuteNonQueryAsync();
            if (headAffected == 0)
            {
                // Headphones row missing
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
            // Delete derived row first (if FK cascade not configured)
            var deleteHeadSql = "DELETE FROM dbo.Headphones WHERE Id = @Id;";
            await using var headCmd = conn.CreateCommand();
            headCmd.Transaction = transaction;
            headCmd.CommandText = deleteHeadSql;
            var hId = headCmd.CreateParameter(); hId.ParameterName = "@Id"; hId.DbType = DbType.Int32; hId.Value = id; headCmd.Parameters.Add(hId);
            await headCmd.ExecuteNonQueryAsync();

            // Delete product
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

    private static Headphone ReadHeadphone(DbDataReader reader)
    {
        var h = new Headphone
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? string.Empty : reader.GetString(reader.GetOrdinal("Name")),
            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? string.Empty : reader.GetString(reader.GetOrdinal("Description")),
            Price = reader.IsDBNull(reader.GetOrdinal("Price")) ? 0.0 : Convert.ToDouble(reader.GetDecimal(reader.GetOrdinal("Price"))),
            ImageFileName = reader.IsDBNull(reader.GetOrdinal("ImageFileName")) ? string.Empty : reader.GetString(reader.GetOrdinal("ImageFileName")),
            Wireless = !reader.IsDBNull(reader.GetOrdinal("Wireless")) && reader.GetBoolean(reader.GetOrdinal("Wireless")),
            Weight = reader.IsDBNull(reader.GetOrdinal("Weight")) ? string.Empty : reader.GetString(reader.GetOrdinal("Weight")),
            Manufacturer = reader.IsDBNull(reader.GetOrdinal("Manufacturer")) ? string.Empty : reader.GetString(reader.GetOrdinal("Manufacturer")),
            Color = reader.IsDBNull(reader.GetOrdinal("Color")) ? string.Empty : reader.GetString(reader.GetOrdinal("Color")),
            Type = reader.IsDBNull(reader.GetOrdinal("Type")) ? string.Empty : reader.GetString(reader.GetOrdinal("Type")),
            BatteryLife = reader.IsDBNull(reader.GetOrdinal("BatteryLife")) ? string.Empty : reader.GetString(reader.GetOrdinal("BatteryLife")),
            NoiseCancellationType = reader.IsDBNull(reader.GetOrdinal("NoiseCancellationType")) ? string.Empty : reader.GetString(reader.GetOrdinal("NoiseCancellationType")),
            Mic = !reader.IsDBNull(reader.GetOrdinal("Mic")) && reader.GetBoolean(reader.GetOrdinal("Mic"))
        };
        return h;
    }
}