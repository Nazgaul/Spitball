<template>
    <div>
        <doc-header></doc-header>
        <div class="item document-wrap">
            <div class="item-content">
                <div class="page text-xs-center" v-for="(page, index) in preview" :key="index">
                    <component class="page-content elevation-1" :is="currentComponent" :src="page"
                               :class="item.contentType+'-content'"></component>
                </div>
            </div>
            <v-flex v-if="accountUser" class="doc-chat-wrapper">
                <a target="_blank" :href="$route.path+'/download'">
                    <div class="download-action-container">
                        <div class="text-wrap">
                            <span class="download-text">Download</span>
                        </div>
                        <div class="btn-wrap">
                            <v-icon class="sb-download-icon">sbf-download-cloud</v-icon>
                        </div>
                    </div>
                </a>
                <div v-show="chatReady" class="chat-title pa-2" v-language:inner>questionDetails_Discussion_Board</div>
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
                // {
                //     id: 'info',
                //     click: function () {
                //     }
                // },
                // {id: 'download'},
                // {id: 'print'},
                // {id: 'more'},
                {id: 'close'}
            ];
            return {
                actions,
                chatReady: false
            }
        },
        props: {
            id: {
                type: String
            }
        },

        methods: {
            ...mapActions([
                'setDocumentPreview',
                'clearDocPreview'
            ]),

            buildChat() {

                if (this.talkSession && this.item) {
                    const otherUserID = this.item.details.user.id;
                    var other1 = new Talk.User(otherUserID);
                    var conversation = this.talkSession.getOrCreateConversation(
                        `document_${this.id}`
                    );
                    //conversation
                    let subject = this.item.details.name.replace(/\r?\n|\r/g, '');
                    conversation.setParticipant(this.chatAccount, {notify: false});
                    conversation.setParticipant(other1);
                    conversation.setAttributes({
                        photoUrl: `${location.origin}/images/conversation.png`,
                        subject: `<${location.href}|${subject}>`
                    });
                    var chatbox = this.talkSession.createChatbox(conversation, {
                        showChatHeader: false
                    });
                    chatbox.on("sendMessage", (t) => {
                        conversation.setParticipant(this.chatAccount, {notify: true})
                    });
                    this.$nextTick(() => {
                        console.log(this.talkSession);
                        chatbox.mount(this.$refs["chat-area"])
                        //doesnt work TODO find a way to handle this error
                            // .then((succ) => {
                            //         if (!!conversation.participants[1].user.__sync) {
                            //             console.log('error')
                            //         } else {
                            //             console.log('good')
                            //         }
                            //
                            //     }, (err) => {
                            //         console.log(err)
                            //
                            //     }
                            // )

                    });
                }
            }
        },

        computed: {
            ...mapGetters(["talkSession", "accountUser", "chatAccount", "getDocumentItem"]),
            currentComponent() {
                if (this.item && this.item.contentType) {
                    return this.item.contentType === "html" ? "iframe" : "img";
                    if (['link', 'text'].find((x) => x == type.toLowerCase())) return 'iframe'
                }
            },
            item() {
                return this.getDocumentItem;
            },
            preview() {
                if (!!this.item && !!this.item.preview) {
                    return this.item.preview
                }

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
            this.setDocumentPreview({type: 'item', id: this.id})
                .then((response) => {
                    self.buildChat();

                });
        },
        //clean store document item on destroy component
        beforeDestroy() {
            console.log('DESTROYYYY!!!');
            this.clearDocPreview();
        }
    }
</script>
<style src="./item.less" lang="less"></style>