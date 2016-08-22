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
           // [ThreadStatic]
            private static Hashtable _localData;
            private static readonly object LocalDataHashtableKey = new object();

            private static Hashtable LocalHashtable
            {

                get
                {
                    if (!RunningInWeb)
                    {
                        //throw new NullReferenceException("You don't suppose to use this method in here since");
                        if (_localData == null)
                        {
                            _localData = new Hashtable();
                        }
                        return _localData;
                    }
                    //if (!RunningInWeb)
                    //{
                    //    //throw new NullReferenceException("You don't suppose to use this method in here since");
                    //    if (_localData != null)
                    //    {
                    //        CallContext.LogicalSetData("LocalData_hash", _localData);
                    //        return _localData;
                    //    }
                    //    var hashTable =  CallContext.LogicalGetData("LocalData_hash") as Hashtable;
                    //    _localData = hashTable ?? new Hashtable();
                    //    CallContext.LogicalSetData("LocalData_hash", _localData);
                    //    return _localData;
                    //}
                    var webHashtable = HttpContext.Current.Items[LocalDataHashtableKey] as Hashtable;
                    if (webHashtable == null)
                    {
                        webHashtable = new Hashtable();
                        HttpContext.Current.Items[LocalDataHashtableKey] = webHashtable;
                    }
                    return webHashtable;
                }
            }

            public object this[object key]
            {
                get
                {
                    if (key == null) throw new ArgumentNullException(nameof(key));
                    return LocalHashtable[key];
                }
                set
                {
                    if (key == null) throw new ArgumentNullException(nameof(key));
                    LocalHashtable[key] = value;
                   
                }
            }

            public int Count => LocalHashtable.Count;

            private static bool RunningInWeb => HttpContext.Current != null;

            public void Clear()
            {
                LocalHashtable.Clear();
            }
        }
    }
}
