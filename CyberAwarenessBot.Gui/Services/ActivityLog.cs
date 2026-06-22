using System;
using System.Collections.Generic;
using System.Linq;
using CyberAwarenessBot.Gui.Models;

namespace CyberAwarenessBot.Gui.Services
{
    public class ActivityLog
    {
        private readonly List<ActivityLogEntry> _entries = new();
        private readonly object _lock = new();

        public void Add(string description)
        {
            lock (_lock)
            {
                _entries.Add(new ActivityLogEntry
                {
                    Timestamp = DateTime.Now,
                    Description = description
                });
            }
        }

        public List<ActivityLogEntry> GetRecent(int count = 10)
        {
            lock (_lock)
            {
                return _entries.OrderByDescending(e => e.Timestamp).Take(count).ToList();
            }
        }

        public List<ActivityLogEntry> GetAll()
        {
            lock (_lock)
            {
                return _entries.OrderByDescending(e => e.Timestamp).ToList();
            }
        }
    }
}
