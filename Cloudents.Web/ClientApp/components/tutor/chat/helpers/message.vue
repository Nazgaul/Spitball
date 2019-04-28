<template>
    <div class="chat-bubble">
        <div class="old-chat-message" :class="[isMine ? 'old-chat-own-message' : 'old-chat-not-own-message' ]">
            <!--<div class="username">{{message.data ? message.data.identity : ''}}</div>-->
            <div class="chat-message-wrap-text" :class="[isMine ? 'old-chat-own-message' : 'old-chat-not-own-message' ]">
                <div class="message-text"
                    v-html="$options.filters.renderUrl(message.data ? message.data.text : '')">
                    <!--{{message.data ? message.data.text : ''}}-->
                </div>

            </div>
        </div>
    </div>
</template>

<script>
    import { mapGetters } from 'vuex'

    export default {
        name: "message.vue",
        props: {
            message: {},
        },
        computed: {
            ...mapGetters({
                'userIdentity': 'userIdentity'
            }),
            isMine() {
                return this.userIdentity === this.message.data.identity
            }
        },

        filters:{
            renderUrl(val){
                let urlRegex =/(\b(https?|ftp|file):\/\/[-A-Z0-9+&@#\/%?=~_|!:,.;]*[-A-Z0-9+&@#\/%=~_|])/ig;
                // Create thumbnail preview if uploaded file link
                if(val.includes('sb-preview_')){
                    let link = val.split('sb-preview_')[1];
                    return '<div class="image-preview">'+ ' <a href="' + link + '" target="_blank">'+ 'File Uploaded'+ '</a> ' +  '<img src="' + link + '">' + '</div>';
                }else{     // regular message with link
                    return val.replace(urlRegex, function(url) {
                        return '<a href="' + url + '" target="_blank">' + 'Link' + '</a>';
                    });
                }
            }
        }

    }
</script>

<style  lang="less">
    .chat-bubble {
        .image-preview{
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            img{
                max-width: 100px;
                height: auto;
            }
        }
        margin-bottom: 8px;
        white-space: pre-wrap;
        word-break: break-all;
        &:first-child{
            margin-top: 4px;
        }
        .username {
            font-size: 10px;
        }
    .old-chat-message {
        display: flex;
        flex-direction: column;
        &.old-chat-own-message {
                align-items: flex-end;
            }
    &.old-chat-not-own-message {
        align-items: flex-start;
    }
        }
    .chat-message-wrap-text {
        display: inline-flex;
        flex-direction: row;
        &.old-chat-own-message{
                justify-items: flex-start;
                background: #f6f6f6;
                font-size: 12px;
                color: rgba(0, 0, 0, 0.87);
                background-attachment: fixed;
                padding: 6px 12px;
                border-radius: 16px;
                .message-text{
                    text-align: left;
                }

            }
    &.old-chat-not-own-message {
        justify-items: flex-end;
        background-color: #e1e1e1;
        font-size: 12px;
        color: rgba(0, 0, 0, 0.87);
        padding: 6px 12px;
        border-radius: 16px;
        .message-text{
                    text-align: right;
                }
            }
        }
    }

</style>