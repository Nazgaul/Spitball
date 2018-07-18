<template>
    <component :is="userId?'router-link':'div'"  :to="userId?{name:'profile',params:{id:userId}}:''">
        <v-avatar tag="v-avatar" size="32" :class="'user-avatar userColor' + userName.length % 11">
        <span class="white--text subheading">{{userName.slice(0,2).toUpperCase()}}</span>
        </v-avatar>
    </component>
</template>
<script>
    export default {
        props:{
            userId:Number,
            userName:{
                type:String
            }
        }
    }
</script>
<style src="./UserAvatar.less" lang="less"></style>
