import twilio from "twilio-chat";
import axios from "axios";


export default {
    /**@type {twilio} **/
    client: {},

    connect:  function() {
        
        var t = axios.post("/chat").then(({ data }) => {
            twilio.create(data).then(c => {
                c.getSubscribedChannels().then(p=> {
                    console.log(p);
                })
                this.client = c;
            });
        });
    },
    sendMessage: function(val) {
       // this.client.se
    }
};