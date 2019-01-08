<template>
    <component v-if="!!userName" :is="userId?'router-link':'div'"  :to="userId?{name:'profile',params:{id:userId}}:''">
        <v-avatar tag="v-avatar"  size="32" :class="'user-avatar userColor' + strToACII % 11">
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
            },
        },
        computed: {
            strToACII() {
                let sum = 0;
                for (let i in this.userName) {
                    sum +=  this.userName.charCodeAt(i);
                }
                return sum
            }
        },
    }
</script>
<style src="./UserAvatar.less" lang="less"></style>
