using System;
using System.Collections;
using System.Runtime.Remoting.Messaging;
using System.Web;
using Zbang.Zbox.Infrastructure.UnitsOfWork;

namespace Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork
{
    public static class Local
    {
        public static ILocalData Data { get; } = new LocalData();

        private class LocalData : ILocalData
        {
            [ThreadStatic]
            private static Hashtable _localData;
            private static readonly object LocalDataHashtableKey = new object();
            //private static readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1);

            private static Hashtable LocalHashtable
            {
                get
                {
                    if (!RunningInWeb)
                    {
                        //SemaphoreSlim.Wait();
                        //throw new NullReferenceException("You don't suppose to use this method in here since");
                        if (_localData != null)
                        {
                            CallContext.LogicalSetData("LocalData_hash", _localData);
                            //SemaphoreSlim.Release();
                            return _localData;
                        }
                        var hashTable = CallContext.LogicalGetData("LocalData_hash") as Hashtable;
                        if (hashTable == null)
                        {
                            _localData = new Hashtable();
                            CallContext.LogicalSetData("LocalData_hash", _localData);
                        }
                        else
                        {
                            _localData = hashTable;
                        }
                        //SemaphoreSlim.Release();
                        return _localData;
                    }
                    var webHashtable = HttpContext.Current.Items[LocalDataHashtableKey] as Hashtable;
                    if (webHashtable == null)
                    {
                        webHashtable = new Hashtable();
                        HttpContext.Current.Items[LocalDataHashtableKey] = webHashtable;
                    }
                    return webHashtable;
                }
            }
          
            public object this[string key]
            {
                get
                {
                    if (key == null) throw new ArgumentNullException(nameof(key));
                    if (RunningInWeb)
                    {
                        return HttpContext.Current.Items[key];
                    }
                    return CallContext.LogicalGetData(key);
                }
                set
                {
                    if (key == null) throw new ArgumentNullException(nameof(key));
                    if (RunningInWeb)
                    {
                        HttpContext.Current.Items[key] = value;
                        return;
                    }
                    CallContext.LogicalSetData(key, value);
                    //if (LocalHashtable[key] != null)
                    //{
                    //    LocalHashtable.Clear();
                    //    //Do something
                    //}
                    //LocalHashtable[key] = value;

                }
            }

            public int Count => LocalHashtable.Count;

            private static bool RunningInWeb => HttpContext.Current != null;

            public void Clear()
            {
                LocalHashtable.Clear();
                if (!RunningInWeb)
                {
                    CallContext.LogicalSetData(UnitOfWork.CurrentUnitOfWorkKey, null);
                    CallContext.LogicalSetData(UnitOfWorkFactory.CurrentSessionKey, null);
                    //CallContext.LogicalSetData("LocalData_hash", null);
                }
            }
        }
    }
}