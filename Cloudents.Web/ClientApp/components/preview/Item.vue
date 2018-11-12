<template>
    <div>
        <doc-header></doc-header>
        <div class="item document-wrap">
            <div class="item-content">
                <v-carousel hide-delimiters hide-controls v-if="$vuetify.breakpoint.smAndDown"
                            style="max-height: 401px;">
                    <v-carousel-item v-for="(page, index) in preview">
                        <div class="page text-xs-center" :key="index">
                            <component class="page-content elevation-1" :is="currentComponent" :src="page"
                                       :class="item.contentType+'-content'"></component>
                        </div>
                    </v-carousel-item>
                </v-carousel>
                <div class="page text-xs-center" v-else v-for="(page, index) in preview" :key="index">
                    <component class="page-content elevation-1" :is="currentComponent" :src="page"
                               :class="item.contentType+'-content'"></component>
                </div>
            </div>
            <v-flex class="doc-chat-wrapper">
                <a target="_blank" @click="downloadDoc()">
                    <div class="download-action-container">
                        <div class="text-wrap">
                            <span class="download-text" v-language:inner>preview_itemActions_download</span>
                            <v-icon class="download-icon-mob ml-2" v-if="$vuetify.breakpoint.smAndDown">
                                sbf-download-cloud
                            </v-icon>

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
    import Talk from "talkjs";

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
                'clearDocPreview',
                'updateLoginDialogState'
            ]),
            downloadDoc() {
                let url = this.$route.path+'/download';
                if(this.accountUser){
                    global.location.href = url;
                }else{
                    this.updateLoginDialogState(true)
                }
            },
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
                    });
                }
            }
        },

        computed: {
            ...mapGetters(["talkSession", "accountUser", "chatAccount", "getDocumentItem"]),
            currentComponent() {
                if (this.item && this.item.contentType) {
                    return this.item.contentType === "html" ? "iframe" : "img";
                    if (['link', 'text'].find((x) => x === type.toLowerCase())) return 'iframe'
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
                    if (this.$vuetify.breakpoint.smAndUp) {
                        self.buildChat();
                    }


                });
        },
        //clean store document item on destroy component
        beforeDestroy() {
            this.clearDocPreview();
        }
    }
</script>
<style src="./item.less" lang="less"></style>