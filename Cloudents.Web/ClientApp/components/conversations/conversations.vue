<template>
    <div>
        <div id="chat-container"></div>
    </div>
</template>

<script>
    import {mapGetters} from 'vuex'

    export default {
        computed: {
            ...mapGetters(["talkSession"]),
        },
        mounted() {
            if (this.talkSession) {
                this.talkSession.syncThemeForLocalDev("/Content/talkjs-theme.css");
                const inbox = this.talkSession.createInbox();
                inbox.mount(document.getElementById("chat-container"));
            }
        },
    };
</script>