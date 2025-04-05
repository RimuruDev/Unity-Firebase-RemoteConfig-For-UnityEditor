#if UNITY_EDITOR
using Firebase;
using UnityEngine;
using System.Reflection;

namespace RimuruDev
{
    public static class FirebaseAppHelper
    {
        // NOTE:
        // В общем не нашел как можно вызвать FirebaseApp.DisposeAllApps() по этому вызываю через рефлекшен.
        // *
        // У меня не получилось грамотно словари подчистить, по этому этот вариант выбрал, что бы наверняка не подтягивать кеши.
        // *
        // Так выглядит метод DisposeAllApps() у класса FirebaseApp:
        //
        // public sealed class FirebaseApp : IDisposable
        // {
        //      internal static void DisposeAllApps()
        //      {
        //          List<FirebaseApp> firebaseAppList = new List<FirebaseApp>();
        //          lock (FirebaseApp.nameToProxy)
        //          {
        //              foreach (KeyValuePair<string, FirebaseApp> keyValuePair in FirebaseApp.nameToProxy)
        //                  firebaseAppList.Add(keyValuePair.Value);
        //          }
        //          foreach (FirebaseApp firebaseApp in firebaseAppList)
        //              firebaseApp.Dispose();
        //      }
        // }
        public static void DisposeAllFirebaseApps()
        {
            var type = typeof(FirebaseApp);

            var method = type.GetMethod("DisposeAllApps", BindingFlags.Static | BindingFlags.NonPublic);

            if (method != null)
                method.Invoke(null, null);
            else
                Debug.LogWarning("<color=red>DisposeAllApps method not found.</color>");
        }
    }
}
#endif