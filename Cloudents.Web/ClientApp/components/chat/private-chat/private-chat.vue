<template>
    <v-flex xs12  class="content-message">

        <div class="header">
            <img src="./../img/user.png" class="avatar" />
            Irma Rogers <span class="availability online"></span></div>
        <div class="container-message">
            <ul v-if="messages.length">
                <li v-for="(msg, index) in messages" :class="{'my-message': msg.fromMe}">
                    <div class="text">
                        <p>{{msg.text}}</p>    
                        <!-- <div class="time">15:0{{index}}</div>                     -->
                    </div>
                    
                </li>                
            </ul>

            <div v-else="!messages.length" class="empty-chat">
                <p>Private chat</p>
                <img src="./../img/start-chat.png"   width="203"  height="170" />
            </div>
        </div>

        <div class="form-message">
            <input type="text" placeholder="Send meessage to seller..." v-model="newMessage" @keyup.enter="sendMsg" />
            <button class="send-btn" @click="sendMsg">Send</button>
        </div>

    </v-flex>
</template>

<style src="./private-chat.less" lang="less"></style>
<script src="./private-chat.js"></script>