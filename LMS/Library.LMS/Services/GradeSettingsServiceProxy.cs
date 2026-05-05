using Library.LMS.Models;

namespace Library.LMS.Services
{
    // Sprint 4: stores custom grade range settings per course
    public class GradeSettingsServiceProxy
    {
        private static GradeSettingsServiceProxy? _instance;
        private static object _instanceLock = new object();

        // courseId -> list of GradeRanges
        private Dictionary<int, List<GradeRange>> _settings;

        private GradeSettingsServiceProxy()
        {
            _settings = new Dictionary<int, List<GradeRange>>();
        }

        public static GradeSettingsServiceProxy Current
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                        _instance = new GradeSettingsServiceProxy();
                }
                return _instance;
            }
        }

        public List<GradeRange> GetRanges(int courseId)
        {
            if (!_settings.ContainsKey(courseId))
                _settings[courseId] = DefaultRanges();
            return _settings[courseId];
        }

        public void SetRanges(int courseId, List<GradeRange> ranges)
        {
            _settings[courseId] = ranges;
        }

        public string GetLetterGrade(int courseId, double percent)
        {
            var ranges = GetRanges(courseId);
            foreach (var r in ranges.OrderByDescending(r => r.MinPercent))
                if (percent >= r.MinPercent && percent <= r.MaxPercent)
                    return r.Letter;
            return "F";
        }

        private List<GradeRange> DefaultRanges() => new List<GradeRange>
        {
            new GradeRange { Letter = "A", MinPercent = 90, MaxPercent = 100, HexColor = "#4CAF50" },
            new GradeRange { Letter = "B", MinPercent = 80, MaxPercent = 89.99, HexColor = "#2196F3" },
            new GradeRange { Letter = "C", MinPercent = 70, MaxPercent = 79.99, HexColor = "#FF9800" },
            new GradeRange { Letter = "D", MinPercent = 60, MaxPercent = 69.99, HexColor = "#FF5722" },
            new GradeRange { Letter = "F", MinPercent = 0,  MaxPercent = 59.99, HexColor = "#F44336" }
        };
    }
}
