<template>
    <div>
        <doc-header></doc-header>
        <div class="item document-wrap">
            <div class="item-content">
                <div class="page text-xs-center" v-for="(page,index) in item.preview" :key="index">
                    <component class="page-content elevation-1" :is="currentComponent" :src="page"
                               :class="item.type+'-content'"></component>
                </div>
            </div>
            <v-flex v-if="accountUser" class="doc-chat-wrapper">
                <div class="chat-title pa-2" v-language:inner>questionDetails_Discussion_Board</div>
                <div ref="chat-area" class="chat-container"></div>
            </v-flex>
        </div>
    </div>
</template>
<script>
    import { mapActions, mapGetters } from 'vuex'
    import docHeader from "./headerDocument.vue"

    export default {
        components: {
            docHeader: docHeader
        },
        data() {
            var actions = [
                {
                    id: 'info',
                    click: function () {
                    }
                },
                {id: 'download'},
                {id: 'print'},
                {id: 'more'},
                {id: 'close'}
            ];
            return {
                item: {},
                actions
            }
        },
        props: {
            id: {
                type: String
            }
        },

        methods: {
            ...mapActions([
                'getPreview',
                'updateItemDetails'
            ]),

            buildChat() {
                if (this.talkSession && this.item) {
                    console.log(this.id)
                    const otherUser = this.item.owner;
                    var other1 = new Talk.User('11');
                    var conversation = this.talkSession.getOrCreateConversation(
                        `document_${this.id}`
                    );
                    //conversation
                    let subject = this.item.name.replace(/\r?\n|\r/g, '');
                    conversation.setParticipant(this.chatAccount, {notify: false});
                    conversation.setParticipant(other1);
                    conversation.setAttributes({
                        photoUrl: `${location.origin}/images/conversation.png`,
                        subject: `<${location.href}|${subject}>`
                    })
                    var chatbox = this.talkSession.createChatbox(conversation, {
                        showChatHeader: false
                    });
                    chatbox.on("sendMessage", (t) => {
                        conversation.setParticipant(this.chatAccount, {notify: true})
                    });
                    this.$nextTick(() => {
                        chatbox.mount(this.$refs["chat-area"]);
                    });
                }
            }
        },

        computed: {
            ...mapGetters(["talkSession", "accountUser", "chatAccount"]),
            currentComponent() {
                return this.item.type === "html" ? "iframe" : "img";
                if (['link', 'text'].find((x) => x == type.toLowerCase())) return 'iframe'
            }
        },
        watch: {
            talkSession: function (newVal, oldVal) {
                if (newVal) {
                    this.buildChat();
                }
            },
        },
        created() {
            let self = this;
            this.getPreview({type: 'item', id: this.id})
                .then(({data: body}) => {
                    self.item = {...body.details, preview: body.preview};
                    self.updateItemDetails({details: body.details});
                    let postfix = self.item.preview[0].split('?')[0].split('.');
                    self.item.type = postfix[postfix.length - 1];
                    self.buildChat();
                })
        },
    }
</script>
<style src="./item.less" lang="less"></style>