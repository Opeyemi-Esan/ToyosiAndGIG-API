using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ToyosiAndGIG.Domain.Entities;
using ToyosiAndGIG.Infrastructure.Data;
using ToyosiAndGIG.Application.Interface;
using ToyosiAndGIG.Domain.Response;

namespace ToyosiAndGIG.Application.Services
{
    //public class SongService
    //{
    //    private readonly ApplicationDbContext _db;

    //    public SongService(ApplicationDbContext db)
    //    {
    //        _db = db;
    //        if (!_db.Songs.Any())
    //        {
    //            _db.Songs.Add(new Song { Title = "Sample Song", Artist = "The Band", Album = "Debut", DurationSeconds = 180, IsDownloadable = false });
    //            _db.SaveChanges();
    //        }
    //    }

    //    public IEnumerable<Song> GetAll() => _db.Songs.OrderByDescending(s => s.UploadedAt).AsNoTracking().ToList();
    //    public Song Get(Guid id) => _db.Songs.Find(id);
    //    public void Add(Song song)
    //    {
    //        _db.Songs.Add(song);
    //        _db.SaveChanges();
    //    }
    //    public bool Update(Song song)
    //    {
    //        var exists = _db.Songs.Find(song.Id);
    //        if (exists == null) return false;
    //        _db.Entry(exists).CurrentValues.SetValues(song);
    //        _db.SaveChanges();
    //        return true;
    //    }
    //    public bool Delete(Guid id)
    //    {
    //        var e = _db.Songs.Find(id);
    //        if (e == null) return false;
    //        _db.Songs.Remove(e);
    //        _db.SaveChanges();
    //        return true;
    //    }
    //    public IEnumerable<Song> Search(string q)
    //    {
    //        q = q?.Trim().ToLower() ?? string.Empty;
    //        if (string.IsNullOrEmpty(q)) return GetAll();
    //        return _db.Songs.AsNoTracking().Where(s => s.Title.ToLower().Contains(q) || (s.Artist ?? string.Empty).ToLower().Contains(q) || (s.Album ?? string.Empty).ToLower().Contains(q)).ToList();
    //    }
    //}

    public class SongService : ISongService
    {
        public Task<Response<string>> DeleteSongAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Response<List<Song>>> GetAllSongsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Response<Song>> GetSongAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Response<Song>> UpdateSongAsync(Song song)
        {
            throw new NotImplementedException();
        }

        public Task<Response<Song>> UploadSongAsync(Song song)
        {
            throw new NotImplementedException();
        }
    }
}
