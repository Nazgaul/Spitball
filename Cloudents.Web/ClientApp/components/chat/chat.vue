<template>
    <v-container py-0 px-0 class="chat-container">
        <v-layout class="chat-header">
            <v-icon>sbf-close</v-icon>
            <span>Messages</span>
        </v-layout>
        <v-layout>
            <component :is="'chat-conversation'"></component>
        </v-layout>
    </v-container>
</template>

<script src="./chat.js"></script>
<style lang="less">
.chat-container{
    position: fixed;
    bottom: 0;
    right: 130px;
    width: 320px;
    .chat-header{
        background-color:#43425d;
        border-radius: 4px 4px 0 0;
        padding:10px;
        color:#fff;
        i{
            color: #a5a4bf;
            font-size:16px;
            margin-right: 14px;
        }
    }
}
</style>
