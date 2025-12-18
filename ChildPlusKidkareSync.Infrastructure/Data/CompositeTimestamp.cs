using ChildPlusKikareSync.Core.Models.ChildPlus;
using Newtonsoft.Json;

namespace ChildPlusKidkareSync.Infrastructure.Data
{
    /// <summary>
    /// Helper class to manage composite timestamps from multiple related tables
    /// </summary>
    public class CompositeTimestamp
    {
        public byte[] MainTableTimestamp { get; set; }
        public Dictionary<string, byte[]> RelatedTablesTimestamps { get; set; } = new();

        /// <summary>
        /// Get the maximum timestamp across all tables
        /// </summary>
        public byte[] GetMaxTimestamp()
        {
            var allTimestamps = new List<byte[]> { MainTableTimestamp };

            if (RelatedTablesTimestamps != null && RelatedTablesTimestamps.Any())
            {
                allTimestamps.AddRange(RelatedTablesTimestamps.Values.Where(v => v != null));
            }

            // Compare byte arrays as BIGINT
            return allTimestamps
                .OrderByDescending(ts => ts != null ? BitConverter.ToInt64(ts, 0) : 0)
                .FirstOrDefault();
        }

        /// <summary>
        /// Get max timestamp as BIGINT for comparison
        /// </summary>
        public long GetMaxTimestampAsLong()
        {
            var maxTimestamp = GetMaxTimestamp();
            return maxTimestamp != null ? BitConverter.ToInt64(maxTimestamp, 0) : 0;
        }

        /// <summary>
        /// Serialize to JSON for storage
        /// </summary>
        public string ToJson()
        {
            var json = new Dictionary<string, string>
        {
            { "MainTable", MainTableTimestamp != null ? BitConverter.ToString(MainTableTimestamp).Replace("-", "") : null }
        };

            if (RelatedTablesTimestamps != null)
            {
                foreach (var (tableName, timestamp) in RelatedTablesTimestamps)
                {
                    json[tableName] = timestamp != null ? BitConverter.ToString(timestamp).Replace("-", "") : null;
                }
            }

            return JsonConvert.SerializeObject(json);
        }

        /// <summary>
        /// Parse from JSON string
        /// </summary>
        public static CompositeTimestamp FromJson(string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            try
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                var composite = new CompositeTimestamp();

                if (dict.TryGetValue("MainTable", out var mainHex) && !string.IsNullOrEmpty(mainHex))
                {
                    composite.MainTableTimestamp = HexStringToByteArray(mainHex);
                }

                foreach (var (key, hexValue) in dict.Where(kvp => kvp.Key != "MainTable"))
                {
                    if (!string.IsNullOrEmpty(hexValue))
                    {
                        composite.RelatedTablesTimestamps[key] = HexStringToByteArray(hexValue);
                    }
                }

                return composite;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Convert hex string to byte array
        /// </summary>
        private static byte[] HexStringToByteArray(string hex)
        {
            if (string.IsNullOrEmpty(hex) || hex == "null")
                return null;

            // Remove "0x" prefix if present
            hex = hex.Replace("0x", "").Replace("0X", "");

            var numberChars = hex.Length;
            var bytes = new byte[numberChars / 2];

            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return bytes;
        }
    }

    // ============================================
    // HELPER EXTENSIONS
    // ============================================

    public static class CompositeTimestampExtensions
    {
        /// <summary>
        /// Create composite timestamp from child and related entities
        /// </summary>
        public static CompositeTimestamp CreateCompositeTimestamp(
            this ChildPlusChild child)
        {
            var composite = new CompositeTimestamp
            {
                MainTableTimestamp = child.Timestamp,
                RelatedTablesTimestamps = new Dictionary<string, byte[]>()
            };

            // Add guardian timestamps
            if (child.Guardians?.Any() == true)
            {
                var maxGuardianTimestamp = child.Guardians
                    .Select(g => g.Timestamp)
                    .Where(ts => ts != null)
                    .OrderByDescending(ts => BitConverter.ToInt64(ts, 0))
                    .FirstOrDefault();

                if (maxGuardianTimestamp != null)
                {
                    composite.RelatedTablesTimestamps["Guardians"] = maxGuardianTimestamp;
                }
            }

            // Add enrollment timestamps
            if (child.Enrollments?.Any() == true)
            {
                var maxEnrollmentTimestamp = child.Enrollments
                    .Select(e => e.Timestamp)
                    .Where(ts => ts != null)
                    .OrderByDescending(ts => BitConverter.ToInt64(ts, 0))
                    .FirstOrDefault();

                if (maxEnrollmentTimestamp != null)
                {
                    composite.RelatedTablesTimestamps["Enrollments"] = maxEnrollmentTimestamp;
                }
            }

            // Add attendance timestamps
            if (child.Attendance?.Any() == true)
            {
                var maxAttendanceTimestamp = child.Attendance
                    .Select(a => a.Timestamp)
                    .Where(ts => ts != null)
                    .OrderByDescending(ts => BitConverter.ToInt64(ts, 0))
                    .FirstOrDefault();

                if (maxAttendanceTimestamp != null)
                {
                    composite.RelatedTablesTimestamps["Attendance"] = maxAttendanceTimestamp;
                }
            }

            return composite;
        }

        /// <summary>
        /// Compare two byte[] timestamps
        /// </summary>
        public static int CompareTimestamps(byte[] ts1, byte[] ts2)
        {
            if (ts1 == null && ts2 == null) return 0;
            if (ts1 == null) return -1;
            if (ts2 == null) return 1;

            var long1 = BitConverter.ToInt64(ts1, 0);
            var long2 = BitConverter.ToInt64(ts2, 0);

            return long1.CompareTo(long2);
        }
    }
}
