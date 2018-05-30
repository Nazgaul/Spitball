<template>
    <div>
        <!--<div>{{finish}}</div>-->
        <div id="chat-container" style="width: 90%; margin: 30px; min-height: 150px"></div>
    </div>
</template>

<script>
    import {mapGetters} from 'vuex'

    export default {
        // watch: {
        //   status: function() {
        //       debugger;
        //     return chatService.client != null;
        //   }
        // },
        data: function () {
            return {
                isReady: false
            };
        },
        computed: {
            ...mapGetters(["talkSession"]),
        },
        mounted() {
            this.isReady = true
            if (this.talkSession && this.isReady) {
                const inbox = this.talkSession.createInbox();
                inbox.mount(document.getElementById("chat-container"));
            }
        },
        // methods: {
        //
        // created: function() {
        //   const inbox = this.talkSession.createInbox();
        //   inbox.mount(document.getElementById("chat-container"));
        // }
        // connect: function() {

        //     var me = new Talk.User()
        //     chatService.connect().then(token => {
        //          const talkSession = new Talk.Session({
        //         appId : "tXsrQpOx",
        //         signature = token
        //     });

        //     });
        //     // const talkSession = new Talk.Session({
        //     //     appId : "tXsrQpOx"
        //     // });
        //     // const inbox = talkSession.createInbox();
        // }
        // }
    };
</script>