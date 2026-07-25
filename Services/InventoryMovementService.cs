using BioGamaEcuador.Data;
using BioGamaEcuador.Models.Sales;
using Microsoft.EntityFrameworkCore;

namespace BioGamaEcuador.Services;

public interface IInventoryService
{
    // Physical product operations
    Task ReserveAsync(Guid productId, int quantity, string reference, string userId, string? notes = null);
    Task ConfirmAsync(Guid productId, int quantity, string reference, string userId, string? notes = null);
    Task ReleaseAsync(Guid productId, int quantity, string reference, string userId, string? notes = null);
    Task EntryAsync(Guid productId, int quantity, string reference, string userId, string? notes = null);
    Task ExitAsync(Guid productId, int quantity, string reference, string userId, string? notes = null);
    Task AdjustAsync(Guid productId, int newStock, string reference, string userId, string notes);
    Task ReturnAsync(Guid productId, int quantity, string reference, string userId, string? notes = null);
    Task TransferAsync(Guid productId, int quantity, Guid origenSucursalId, Guid destinoSucursalId, string reference, string userId, string? notes = null);
    // Course seat operations
    Task LogReservationAsync(Guid courseId, int quantity, string reference, string userId, string? notes = null);
    Task LogConfirmationAsync(Guid courseId, int quantity, string reference, string userId, string? notes = null);
    Task LogReleaseAsync(Guid courseId, int quantity, string reference, string userId, string? notes = null);
}

public sealed class InventoryMovementService(AppDbContext context) : IInventoryService
{
    // ── Physical product operations ──────────────────────────────────
    public Task ReserveAsync(Guid id, int qty, string reference, string userId, string? notes = null) => ChangeAsync(id, qty, "Reserva", reference, userId, notes, p => { if (p.Stock - p.ReservedStock < qty) throw new InvalidOperationException("Stock disponible insuficiente."); p.ReservedStock += qty; });
    public Task ConfirmAsync(Guid id, int qty, string reference, string userId, string? notes = null) => ChangeAsync(id, qty, "Confirmacion", reference, userId, notes, p => { if (p.ReservedStock < qty || p.Stock < qty) throw new InvalidOperationException("No existe stock reservado suficiente para confirmar."); p.ReservedStock -= qty; p.Stock -= qty; });
    public Task ReleaseAsync(Guid id, int qty, string reference, string userId, string? notes = null) => ChangeAsync(id, qty, "Liberacion", reference, userId, notes, p => { if (p.ReservedStock < qty) throw new InvalidOperationException("No existe stock reservado suficiente para liberar."); p.ReservedStock -= qty; });
    public Task EntryAsync(Guid id, int qty, string reference, string userId, string? notes = null) => ChangeAsync(id, qty, "Entrada", reference, userId, notes, p => p.Stock += qty);
    public Task ExitAsync(Guid id, int qty, string reference, string userId, string? notes = null) => ChangeAsync(id, qty, "Salida", reference, userId, notes, p => { if (p.Stock - p.ReservedStock < qty) throw new InvalidOperationException("Stock disponible insuficiente."); p.Stock -= qty; });
    public async Task AdjustAsync(Guid id, int newStock, string reference, string userId, string notes)
    {
        var product = await ProductAsync(id);
        if (newStock < product.ReservedStock) throw new InvalidOperationException("El stock no puede ser menor al stock reservado.");
        var previous = product.Stock; var difference = Math.Abs(newStock - previous);
        if (difference == 0) return;
        product.Stock = newStock; product.UpdatedAt = DateTime.UtcNow;
        context.InventoryMovements.Add(Movement(product, "Ajuste", difference, previous, newStock, reference, userId, notes));
        await context.SaveChangesAsync();
    }
    public async Task ReturnAsync(Guid id, int qty, string reference, string userId, string? notes = null)
    {
        var product = await ProductAsync(id); var previous = product.Stock;
        product.Stock += qty; product.UpdatedAt = DateTime.UtcNow;
        context.InventoryMovements.Add(Movement(product, "Devolucion", qty, previous, product.Stock, reference, userId, notes));
        await context.SaveChangesAsync();
    }

    public async Task TransferAsync(Guid id, int qty, Guid origenSucursalId, Guid destinoSucursalId, string reference, string userId, string? notes = null)
    {
        var product = await ProductAsync(id); var previous = product.Stock;
        if (product.Stock - product.ReservedStock < qty) throw new InvalidOperationException("Stock disponible insuficiente para transferir.");
        product.Stock -= qty; product.UpdatedAt = DateTime.UtcNow;
        context.InventoryMovements.Add(new InventoryMovement
        {
            PhysicalProductId = id,
            SucursalId = origenSucursalId,
            TipoMovimiento = "Transferencia",
            Cantidad = qty,
            StockAnterior = previous,
            StockPosterior = product.Stock,
            Referencia = $"{reference}|origen:{origenSucursalId}|destino:{destinoSucursalId}",
            UsuarioId = userId,
            Observacion = notes ?? $"Transferencia de {origenSucursalId} a {destinoSucursalId}"
        });
        await context.SaveChangesAsync();
    }

    private async Task ChangeAsync(Guid id, int qty, string type, string reference, string userId, string? notes, Action<PhysicalProduct> change)
    {
        var product = await ProductAsync(id); var previous = product.Stock;
        change(product); product.UpdatedAt = DateTime.UtcNow;
        context.InventoryMovements.Add(Movement(product, type, qty, previous, product.Stock, reference, userId, notes));
        await context.SaveChangesAsync();
    }
    private async Task<PhysicalProduct> ProductAsync(Guid id) => await context.PhysicalProducts.SingleOrDefaultAsync(p => p.Id == id) ?? throw new KeyNotFoundException("Producto no encontrado.");
    private static InventoryMovement Movement(PhysicalProduct p, string type, int qty, int previous, int next, string reference, string userId, string? notes) => new() { PhysicalProductId = p.Id, TipoMovimiento = type, Cantidad = qty, StockAnterior = previous, StockPosterior = next, Referencia = reference, UsuarioId = userId, Observacion = notes ?? string.Empty };

    // ── Course seat operations ───────────────────────────────────────
    public async Task LogReservationAsync(Guid courseId, int quantity, string reference, string userId, string? notes = null)
    {
        var course = await CourseAsync(courseId);
        context.InventoryMovements.Add(new InventoryMovement
        {
            CourseId = courseId,
            TipoMovimiento = "Reserva",
            Cantidad = quantity,
            StockAnterior = course.ReservedSeats - quantity,
            StockPosterior = course.ReservedSeats,
            Referencia = reference,
            UsuarioId = userId,
            Observacion = notes ?? string.Empty
        });
        await context.SaveChangesAsync();
    }
    public async Task LogConfirmationAsync(Guid courseId, int quantity, string reference, string userId, string? notes = null)
    {
        var course = await CourseAsync(courseId);
        context.InventoryMovements.Add(new InventoryMovement
        {
            CourseId = courseId,
            TipoMovimiento = "Confirmacion",
            Cantidad = quantity,
            StockAnterior = course.ConfirmedSeats - quantity,
            StockPosterior = course.ConfirmedSeats,
            Referencia = reference,
            UsuarioId = userId,
            Observacion = notes ?? string.Empty
        });
        await context.SaveChangesAsync();
    }
    public async Task LogReleaseAsync(Guid courseId, int quantity, string reference, string userId, string? notes = null)
    {
        var course = await CourseAsync(courseId);
        context.InventoryMovements.Add(new InventoryMovement
        {
            CourseId = courseId,
            TipoMovimiento = "Liberacion",
            Cantidad = quantity,
            StockAnterior = course.ReservedSeats + quantity,
            StockPosterior = course.ReservedSeats,
            Referencia = reference,
            UsuarioId = userId,
            Observacion = notes ?? string.Empty
        });
        await context.SaveChangesAsync();
    }
    private async Task<Course> CourseAsync(Guid id) => await context.Courses.AsNoTracking().SingleOrDefaultAsync(p => p.Id == id) ?? throw new KeyNotFoundException("Curso no encontrado.");
}
