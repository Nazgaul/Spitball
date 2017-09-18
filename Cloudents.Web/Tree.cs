using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cloudents.Web
{
    public class TreeNode<V> 
    {
        public TreeNode(V value)
        {
            Value = value;
        }

        public V Value { get;private set; }
        public LinkedList<TreeNode<V>> Children { get; private set; }

        public void AddChild(V data)
        {
            Children.AddFirst(new TreeNode<V>(data));
        }

        //public void Traverse(TreeNode<T> node, TreeVisitor<T> visitor)
        //{
        //    visitor(node.data);
        //    foreach (NTree<T> kid in node.children)
        //        Traverse(kid, visitor);
        //}


        public override string ToString()
        {

            return Value.ToString();

        }

        
    }


    public class TreeConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                //JToken t = JToken.FromObject(value);

                //var p = new JObject();

                //if (t.Type != JTokenType.Object)
                //{
                //    t.WriteTo(writer);
                //}
                //else
                //{

                //    JObject o = (JObject)t;
                //    IList<string> propertyNames = o.Properties().Select(p => p.Name).ToList();

                //    o.AddFirst(new JProperty("Keys", new JArray(propertyNames)));

                //    o.WriteTo(writer);
                //}
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
            }
            public override bool CanRead => false;

            public override bool CanConvert(Type objectType)
            {
                return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(TreeNode<>);
            }
        }
    }