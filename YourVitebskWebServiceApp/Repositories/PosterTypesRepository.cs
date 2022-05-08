﻿using System;
using System.Collections.Generic;
using System.Linq;
using YourVitebskWebServiceApp.Interfaces;
using YourVitebskWebServiceApp.Models;

namespace YourVitebskWebServiceApp.Repositories
{
    public class PosterTypesRepository : IRepository<PosterType>
    {
        private readonly YourVitebskDBContext _context;
        private bool _disposed = false;

        public PosterTypesRepository(YourVitebskDBContext context)
        {
            _context = context;
        }

        public IEnumerable<IViewModel> Get()
        {
            return _context.PosterTypes.ToList().OrderBy(x => x.PosterTypeId);
        }

        public PosterType Get(int id)
        {
            return _context.PosterTypes.FirstOrDefault(x => x.PosterTypeId == id);
        }

        public void Create(PosterType posterType)
        {
            _context.PosterTypes.Add(posterType);
            _context.SaveChanges();
        }

        public void Update(PosterType posterType)
        {
            _context.PosterTypes.Update(posterType);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            _context.PosterTypes.Remove(Get(id));
            _context.SaveChanges();
        }

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}