<template>
    <v-avatar size="32" :class="'user-avatar userColor' + userName.length % 11">
        <span class="white--text">{{userName.slice(0,2).toUpperCase()}}</span>
    </v-avatar>
</template>
<script>
    export default {
        props:{
            userName:{
                type:String
            }
        }
    }
</script>
<style src="./UserAvatar.less" lang="less"></style>
