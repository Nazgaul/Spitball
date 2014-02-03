using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Events;

namespace Zbang.Zbox.Domain.Events
{
    public class BoxItemCreatedEvent : BaseEvent
    {
        //Fields
        //readonly Type m_boxItem;
       // readonly string m_BoxitemName;
       // private readonly string m_BoxItemUrl;

        //Ctor
        public BoxItemCreatedEvent(string emailId, long boxId/*, Type boxItem, string boxItemName/*,string boxItemUrl*/): base(emailId, boxId)
        {
           // m_boxItem = boxItem;
           // m_BoxitemName = boxItemName;
            //m_BoxItemUrl = boxItemUrl;
        }

        //Properties
        //public string boxItem
        //{
        //    get
        //    {
        //        return m_boxItem.Name;
        //    }
        //}

        //public string BoxItemName 
        //{ 
        //    get 
        //    { 
        //        return m_BoxitemName; 
        //    } 
        //}
        
        //public string boxItemUrl
        //{
        //    get 
        //    { 
        //        return m_BoxItemUrl; 
        //    }          
        //}        
    }
}
