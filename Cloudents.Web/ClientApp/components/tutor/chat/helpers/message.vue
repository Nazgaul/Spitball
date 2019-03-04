<template>
    <div>
        <div class="message" v-for="(message,index) in messages" :class="{own: message.user == username}">
            <!--<div class="username" v-if="index>0">{{message.user}}</div>-->
            <div class="username">{{message.user}}</div>
            <div style="margin-top: 5px"></div>
            <!--<div class="content">-->
                <!--<div v-html="message.content"></div>-->
                <!--<chat-image v-if="message.image" ></chat-image>-->
            <!--</div>-->
        </div>
    </div>
</template>

<script>
    export default {
        name: "message.vue",
        props: {
            messages: {

            },
        },

    }
</script>

<style scoped>

</style>