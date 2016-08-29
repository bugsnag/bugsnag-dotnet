using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;

namespace Bugsnag
{
    internal class OfflineStore
    {
        private static object _lock = new object();

        internal IEnumerable<string> ReadStoredJson()
        {
            using (var store = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null))
            {
                store.CreateDirectory("crash_reports");
                foreach (var filePath in store.GetFileNames("crash_reports\\*"))
                {
                    string fileData = ReadAndRemoveCrashReport(store, filePath);

                    if (fileData != null)
                    {
                        yield return fileData;
                    }
                }
            }
        }

        internal void StoreJson(string json)
        {
            using (var store = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null))
            {
                store.CreateDirectory("crash_reports");
                using (var storageStream = new IsolatedStorageFileStream(string.Format("crash_reports\\{0}.json", Guid.NewGuid()), FileMode.CreateNew, store))
                {
                    var writer = new StreamWriter(storageStream);
                    writer.Write(json);
                    writer.Flush();
                }
            }
        }

        private string ReadAndRemoveCrashReport(IsolatedStorageFile store, string filePath)
        {
            string fileData = null;

            lock (_lock)
            {
                try
                {
                    using (var storageStream = new IsolatedStorageFileStream(string.Format("crash_reports\\{0}", filePath), FileMode.Open, store))
                    {
                        var reader = new StreamReader(storageStream);
                        fileData = reader.ReadToEnd();
                    }
                }
                catch (FileNotFoundException)
                {
                    // we can assume here that this crash report has already been sent in another thread
                }
                store.DeleteFile(string.Format("crash_reports\\{0}", filePath));
            }

            return fileData;
        }
    }
}
