﻿using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Cloudents.Core.Enum;
using Newtonsoft.Json;
using Skarp.HubSpotClient;
using Skarp.HubSpotClient.Contact;
using Skarp.HubSpotClient.Contact.Dto;

namespace Cloudents.Infrastructure
{
    public class HubSpotClient
    {
        private readonly HubSpotContactClient _api = new HubSpotContactClient("57453297-0104-4d83-8a3c-e58588c15a90");

        public async Task<HubSpotContact> GetContactByEmailAsync(string email)
        {
            var timeToWaitInMillisecond = 5;
            do
            {
                try
                {
                    var contact = await _api.GetByEmailAsync<HubSpotContact>(email);
                    return contact;
                }
               
                catch(HubSpotException e)
                {
                    //502 error -  https://github.com/skarpdev/dotnetcore-hubspot-client/pull/30
                    if (e.Message.StartsWith("<"))
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(timeToWaitInMillisecond));
                        timeToWaitInMillisecond = timeToWaitInMillisecond * 2;
                        continue;
                    }
                    dynamic response = JsonConvert.DeserializeObject(e.RawJsonResponse);
                    if (response.errorType == "RATE_LIMIT")
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(timeToWaitInMillisecond));
                        timeToWaitInMillisecond = timeToWaitInMillisecond * 2;
                    }
                    else
                    {
                        throw;
                    }
                }
            } while (true);
        }

       

        
 
        public async Task CreateOrUpdateAsync(HubSpotContact contact, bool needInsert)
        {
            var timeToWaitInMillisecond = 5;
            do
            {
                try
                {
                    if (needInsert)
                    {
                        await _api.CreateAsync<HubSpotContact>(contact);
                        return;
                    }

                    await _api.UpdateAsync(contact);
                    return;
                }
                catch (HubSpotException e)
                {
                    if (e.Message.StartsWith("<"))
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(timeToWaitInMillisecond));
                        timeToWaitInMillisecond = timeToWaitInMillisecond * 2;
                        continue;
                    }
                    dynamic response = JsonConvert.DeserializeObject(e.RawJsonResponse);
                    if (response.errorType == "RATE_LIMIT")
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(timeToWaitInMillisecond));
                        timeToWaitInMillisecond = timeToWaitInMillisecond * 2;
                       
                    }
                    else
                    {
                        throw;
                    }

                }
             
            } while (true);
        }
    }

     [DataContract]
    public class HubSpotContact : ContactHubSpotEntity
    {
        public ItemState _status;
        private DateTimeOffset _registrationDate;

        public void SetRegistrationTime(DateTime d)
        {

            _registrationDate = d.ToUniversalTime().Date;

        }


        [DataMember(Name = "teacher_id")]
        public long SpitballId { get; set; }

        [DataMember(Name = "registration_date")]
        public long RegistrationDate
        {
            get { return _registrationDate.ToUnixTimeMilliseconds(); }
            set { }
        }

        [DataMember(Name = "country")]
        public string Country { get; set; } //Need to figure out

        [DataMember(Name = "teacher_status")]
        public string Status
        {
            get
            {
                switch (_status)
                {
                    case ItemState.Ok:
                        return "Approved";

                    case ItemState.Pending:
                        return "Pending";
                    case ItemState.Flagged:
                        return "Suspended";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set { }
        }

        [DataMember(Name = "bio")]
        public string Bio { get; set; }

        [DataMember(Name = "headline")]
        public string? Title { get; set; }

        [DataMember(Name = "courses")]
        public string Courses { get; set; }  // NEED to figure out



        [IgnoreDataMember]
        public bool Payment { get; set; }

        [DataMember(Name = "has_payment")]
        public string PaymentStr
        {
            get { return Payment.ToString().ToLowerInvariant(); }
            set { }
        }


        [DataMember(Name = "has_subscription")]
        public string HasSubscriptionStr
        {
            get { return SubscriptionPrice.HasValue.ToString().ToLowerInvariant(); }
            set { }
        }

        [IgnoreDataMember]
        public double? SubscriptionPrice { get; set; }

        [DataMember(Name = "subscription_price")]
        public string SubscriptionPriceStr
        {
            get
            {
                if (SubscriptionPrice.HasValue)
                {
                    SubscriptionPrice.Value.ToString("C0");
                }
                return SubscriptionPrice.ToString();
            }
            set { }
        }

        [DataMember(Name = "number_of_documents")]
        public int DocumentCount { get; set; }

        [DataMember(Name = "number_of_followers")]
        public int FollowerCount { get; set; }

        [DataMember(Name = "number_of_lessons")]
        public int LessonsCount { get; set; }


        [IgnoreDataMember]
        public double Rate { get; set; }


        [DataMember(Name = "avg_rating")]
        public string RateToSend
        {
            get
            {
                var avg = (Math.Round(Rate * 2, MidpointRounding.AwayFromZero) / 2);
                if (Math.Abs(avg) < 0.1)
                {
                    return null;
                }
                return (Math.Round(Rate * 2, MidpointRounding.AwayFromZero) / 2).ToString("F1");
            }
            set
            {
                //DONT CARE
            }
        }


        [DataMember(Name = "number_of_live_classes_scheduled")]
        public int LiveScheduled { get; set; }
        [DataMember(Name = "number_of_live_classes_done")]
        public int LiveDone { get; set; }
        [DataMember(Name = "number_of_private_classes_scheduled")]
        public int PrivateScheduled { get; set; }
        [DataMember(Name = "number_of_private_classes_done")]
        public int PrivateDone { get; set; }


        [DataMember(Name = "hubspot_owner_id")]
        public string OwnerId { get; set; }

    }
}
