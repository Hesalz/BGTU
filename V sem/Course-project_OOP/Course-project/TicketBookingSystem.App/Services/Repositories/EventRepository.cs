using System.Linq;
using Microsoft.EntityFrameworkCore;
using TicketBookingSystem.Data;
using TicketBookingSystem.Models;

namespace TicketBookingSystem.Services.Repositories;

public class EventRepository : Repository<Event>, IEventRepository
{
    public EventRepository(ApplicationDbContext context) : base(context)
    {
    }

    private IQueryable<Event> QueryWithPhotos()
    {
        // Используем AsNoTracking для ускорения чтения и не тянем связанные билеты,
        // так как они подгружаются отдельно через TicketRepository
        return _dbSet
            .AsNoTracking()
            .Include(e => e.Photos);
    }

    public override async Task<IEnumerable<Event>> GetAllAsync()
    {
        return await QueryWithPhotos().ToListAsync();
    }

    public override async Task<Event?> GetByIdAsync(int id)
    {
        return await QueryWithPhotos().FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Event?> GetByIdForUpdateAsync(int id)
    {
        // Проверяем, не отслеживается ли уже Event с этим Id
        var trackedEvent = _context.ChangeTracker.Entries<Event>()
            .FirstOrDefault(e => e.Entity.Id == id);
        
        if (trackedEvent != null)
        {
            // Если Event уже отслеживается, загружаем Photos если они еще не загружены
            if (!_context.Entry(trackedEvent.Entity).Collection(e => e.Photos).IsLoaded)
            {
                await _context.Entry(trackedEvent.Entity)
                    .Collection(e => e.Photos)
                    .LoadAsync();
            }
            return trackedEvent.Entity;
        }
        
        // Если Event не отслеживается, загружаем его с отслеживанием
        return await _dbSet
            .Include(e => e.Photos)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Event?> GetByIdForUpdateWithoutPhotosAsync(int id)
    {
        // Загружаем событие с отслеживанием для обновления без Photos
        // Используется когда Photos обрабатываются отдельно через ReplaceEventPhotosAsync
        return await _dbSet
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Event>> GetUpcomingEventsAsync()
    {
        var now = DateTime.Now; // Используем локальное время для сравнения
        var events = await QueryWithPhotos()
            .ToListAsync();

        // Фильтруем мероприятия, которые еще не прошли (учитываем дату и время)
        // Объединяем Date и Time для корректного сравнения
        return events
            .Where(e =>
            {
                // Объединяем дату и время мероприятия
                // e.Date может содержать время, поэтому берем только дату и добавляем Time
                var eventDateOnly = e.Date.Date;
                var eventDateTime = eventDateOnly.Add(e.Time);
                // Сравниваем с текущим локальным временем
                // Используем строгое сравнение: мероприятие должно быть в будущем
                var isUpcoming = eventDateTime > now;
                return isUpcoming;
            })
            .OrderBy(e => e.Date)
            .ThenBy(e => e.Time);
    }

    public async Task<IEnumerable<Event>> SearchEventsAsync(string searchTerm)
    {
        var term = searchTerm.ToLower();
        return await QueryWithPhotos()
            .Where(e => e.Title.ToLower().Contains(term) ||
                       e.Description.ToLower().Contains(term) ||
                       e.Location.ToLower().Contains(term))
            .OrderBy(e => e.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var events = await QueryWithPhotos()
            .Where(e => e.Date.Date >= startDate.Date && e.Date.Date <= endDate.Date)
            .ToListAsync();

        return events
            .OrderBy(e => e.Date)
            .ThenBy(e => e.Time);
    }

    public override async Task DeleteAsync(int id)
    {
        // Проверяем, не отслеживается ли уже Event с этим Id
        var trackedEvent = _context.ChangeTracker.Entries<Event>()
            .FirstOrDefault(e => e.Entity.Id == id);

        Event? entityToDelete;
        
        if (trackedEvent != null)
        {
            // Если Event уже отслеживается, используем его
            entityToDelete = trackedEvent.Entity;
            
            // Отсоединяем связанные Photos, если они загружены
            var trackedPhotos = _context.ChangeTracker.Entries<EventPhoto>()
                .Where(e => e.Entity.EventId == id)
                .ToList();
            
            foreach (var photoEntry in trackedPhotos)
            {
                photoEntry.State = EntityState.Detached;
            }
        }
        else
        {
            // Если Event не отслеживается, загружаем его БЕЗ отслеживания, затем создаем новый для удаления
            var eventFromDb = await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);
            
            if (eventFromDb == null)
            {
                return; // Event не найден, ничего не делаем
            }
            
            // Создаем новый экземпляр для удаления, чтобы избежать конфликтов отслеживания
            entityToDelete = new Event { Id = eventFromDb.Id };
            _context.Entry(entityToDelete).State = EntityState.Deleted;
        }

        if (entityToDelete != null)
        {
            // Если сущность еще не помечена как удаленная
            var entry = _context.Entry(entityToDelete);
            if (entry.State != EntityState.Deleted)
            {
                _dbSet.Remove(entityToDelete);
            }
            await _context.SaveChangesAsync();
        }
    }

    public override async Task UpdateAsync(Event entity)
    {
        // Используем более безопасный способ обновления
        // Так как entity уже отслеживается (загружена через GetByIdForUpdateWithoutPhotosAsync),
        // мы просто обновляем свойства напрямую, и EF автоматически обнаружит изменения
        // Это предотвращает конфликты с навигационными свойствами (Photos, Tickets)
        var entry = _context.Entry(entity);
        
        // Убеждаемся, что сущность отслеживается
        if (entry.State == EntityState.Detached)
        {
            // Если сущность не отслеживается, находим существующую и обновляем её
            var existing = await _dbSet.FindAsync(entity.Id);
            if (existing != null)
            {
                // Копируем только скалярные свойства, не трогая навигационные
                existing.Title = entity.Title;
                existing.Description = entity.Description;
                existing.Date = entity.Date;
                existing.Time = entity.Time;
                existing.Location = entity.Location;
                existing.TotalTickets = entity.TotalTickets;
                existing.Price = entity.Price;
                existing.Categories = entity.Categories;
                existing.UpdatedAt = entity.UpdatedAt;
            }
            else
            {
                throw new InvalidOperationException($"Event with id {entity.Id} not found");
            }
        }
        // Если сущность уже отслеживается, свойства уже обновлены в EventService,
        // и EF автоматически обнаружит изменения при SaveChangesAsync
        
        await _context.SaveChangesAsync();
    }

    public async Task ReplaceEventPhotosAsync(int eventId, IEnumerable<byte[]> photoData)
    {
        var data = photoData?.Where(p => p is { Length: > 0 }).ToList() ?? new List<byte[]>();

        // Отсоединяем все отслеживаемые EventPhoto для этого события перед удалением
        // Это предотвращает конфликт, если Photos уже загружены с отслеживанием
        var trackedPhotos = _context.ChangeTracker.Entries<EventPhoto>()
            .Where(e => e.Entity.EventId == eventId)
            .ToList();
        
        foreach (var entry in trackedPhotos)
        {
            entry.State = EntityState.Detached;
        }

        // Удаляем существующие фотографии напрямую в базе данных, минуя отслеживание
        // ExecuteDeleteAsync выполняется напрямую в БД и не требует SaveChangesAsync
        await _context.EventPhotos
            .Where(p => p.EventId == eventId)
            .ExecuteDeleteAsync();

        if (data.Count > 0)
        {
            var newPhotos = data.Select((bytes, index) => new EventPhoto
            {
                EventId = eventId,
                ImageData = bytes,
                SortOrder = index
            }).ToList();

            await _context.EventPhotos.AddRangeAsync(newPhotos);
            // Сохраняем только новые фотографии, так как ExecuteDeleteAsync уже выполнил удаление
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Event> UpdateEventWithPhotosAsync(Event eventEntity, IEnumerable<byte[]> photoData)
    {
        // Объединяем обновление Event и Photos в одну транзакцию для атомарности и предотвращения конфликтов
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Проверяем, не отслеживается ли уже Event с этим Id
            var trackedEvent = _context.ChangeTracker.Entries<Event>()
                .FirstOrDefault(e => e.Entity.Id == eventEntity.Id);
            
            Event existingEvent;
            if (trackedEvent != null)
            {
                // Если Event уже отслеживается, используем его
                existingEvent = trackedEvent.Entity;
            }
            else
            {
                // Если Event не отслеживается, загружаем его
                existingEvent = await _dbSet.FindAsync(eventEntity.Id) 
                    ?? throw new InvalidOperationException($"Event with id {eventEntity.Id} not found");
            }

            // Обновляем скалярные свойства
            existingEvent.Title = eventEntity.Title;
            existingEvent.Description = eventEntity.Description;
            existingEvent.Date = eventEntity.Date;
            existingEvent.Time = eventEntity.Time;
            existingEvent.Location = eventEntity.Location;
            existingEvent.TotalTickets = eventEntity.TotalTickets;
            existingEvent.Price = eventEntity.Price;
            existingEvent.Categories = eventEntity.Categories;
            existingEvent.UpdatedAt = eventEntity.UpdatedAt;

            // Обновляем фотографии
            var data = photoData?.Where(p => p is { Length: > 0 }).ToList() ?? new List<byte[]>();

            // Загружаем существующие фотографии для удаления
            var existingPhotos = await _context.EventPhotos
                .Where(p => p.EventId == eventEntity.Id)
                .ToListAsync();

            // Удаляем старые фотографии
            if (existingPhotos.Any())
            {
                _context.EventPhotos.RemoveRange(existingPhotos);
            }

            // Добавляем новые фотографии
            if (data.Count > 0)
            {
                var newPhotos = data.Select((bytes, index) => new EventPhoto
                {
                    EventId = eventEntity.Id,
                    ImageData = bytes,
                    SortOrder = index
                }).ToList();

                await _context.EventPhotos.AddRangeAsync(newPhotos);
            }

            // Сохраняем все изменения одной операцией
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            // Отсоединяем Event от контекста перед возвратом, чтобы избежать конфликтов
            // при последующих операциях с этим же контекстом
            _context.Entry(existingEvent).State = EntityState.Detached;
            
            // Загружаем Event БЕЗ отслеживания для возврата, чтобы избежать конфликтов
            var result = await QueryWithPhotos()
                .FirstOrDefaultAsync(e => e.Id == existingEvent.Id);
            
            return result ?? existingEvent;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}


