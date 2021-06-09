using CooverBoxWebApplication.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CooverBoxWebApplication.Services
{
    public class MaterialsService
    {
        private readonly DBAppContext _dbContext;
        private readonly IMemoryCache _memoryCache;

        public MaterialsService(DBAppContext context, IMemoryCache cache)
        {
            _dbContext = context;
            _memoryCache = cache;
        }
        public IEnumerable<ArtHay> ArtHays
        {
            get
            {
                return _memoryCache.GetOrCreate("ArtHays", entry =>
                {
                    entry.SetSlidingExpiration(TimeSpan.FromMinutes(30));
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6);
                    return _dbContext.ArtHays.ToList();
                });
            }
        }
        public IEnumerable<Cloth> Cloths
        {
            get
            {
                return _memoryCache.GetOrCreate("Cloths", entry =>
                {
                    entry.SetSlidingExpiration(TimeSpan.FromMinutes(30));
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6);
                    return _dbContext.Cloths.ToList();
                });
            }
        }

        public IEnumerable<CoverCarton> CoverCartons
        {
            get
            {
                return _memoryCache.GetOrCreate("CoverCartons", entry =>
                {
                    entry.SetSlidingExpiration(TimeSpan.FromMinutes(30));
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6);
                    return _dbContext.CoverCartons.ToList();
                });
            }
        }
        public IEnumerable<DesignPaper> DesignPapers
        {
            get
            {
                return _memoryCache.GetOrCreate("DesignPapers", entry =>
                {
                    entry.SetSlidingExpiration(TimeSpan.FromMinutes(30));
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6);
                    return _dbContext.DesignPapers.ToList();
                });
            }
        }
        public IEnumerable<FringePaper> FringePapers
        {
            get
            {
                return _memoryCache.GetOrCreate("FringePapers", entry =>
                {
                    entry.SetSlidingExpiration(TimeSpan.FromMinutes(30));
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6);
                    return _dbContext.FringePapers.ToList();
                });
            }
        }
        public IEnumerable<Grommet> Grommets
        {
            get
            {
                return _memoryCache.GetOrCreate("Grommets", entry =>
                {
                    entry.SetSlidingExpiration(TimeSpan.FromMinutes(30));
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6);
                    return _dbContext.Grommets.ToList();
                });
            }
        }
        public IEnumerable<Isolon> Isolons
        {
            get
            {
                return _memoryCache.GetOrCreate("Isolons", entry =>
                {
                    entry.SetSlidingExpiration(TimeSpan.FromMinutes(30));
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6);
                    return _dbContext.Isolons.ToList();
                });
            }
        }
        public IEnumerable<Magnet> Magnets
        {
            get
            {
                return _memoryCache.GetOrCreate("Magnets", entry =>
                {
                    entry.SetSlidingExpiration(TimeSpan.FromMinutes(30));
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6);
                    return _dbContext.Magnets.ToList();
                });
            }
        }
        public IEnumerable<Ribbon> Ribbons
        {
            get
            {
                return _memoryCache.GetOrCreate("Ribbons", entry =>
                {
                    entry.SetSlidingExpiration(TimeSpan.FromMinutes(30));
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6);
                    return _dbContext.Ribbons.ToList();
                });
            }
        }
        public IEnumerable<Rubber> Rubbers
        {
            get
            {
                return _memoryCache.GetOrCreate("Rubbers", entry =>
                {
                    entry.SetSlidingExpiration(TimeSpan.FromMinutes(30));
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6);
                    return _dbContext.Rubbers.ToList();
                });
            }
        }
        public IEnumerable<TissuePaper> TissuePapers
        {
            get
            {
                return _memoryCache.GetOrCreate("TissuePapers", entry =>
                {
                    entry.SetSlidingExpiration(TimeSpan.FromMinutes(30));
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6);
                    return _dbContext.TissuePapers.ToList();
                });
            }
        }
        public IEnumerable<Cord> Cords
        {
            get
            {
                return _memoryCache.GetOrCreate("Cords", entry =>
                {
                    entry.SetSlidingExpiration(TimeSpan.FromMinutes(30));
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6);
                    return _dbContext.Cords.ToList();
                });
            }
        }
    }
}
