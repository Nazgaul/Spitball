<template>
    <component v-if="!!userName" :is="userId?'router-link':'div'" :to="userId?{name:'profile',params:{id:userId}}:''">
        <v-avatar v-if="isImage"  tag="v-avatar" size="32" :class="'user-avatar image'">
            <img :src="userImageUrl" alt="user avatar">
            <!--<span class="white&#45;&#45;text font-14">{{userName.slice(0,2).toUpperCase()}}</span>-->
        </v-avatar>

        <v-avatar v-else tag="v-avatar" size="32" :class="'user-avatar userColor' + strToACII % 11">
            <span class="white--text font-14">{{userName.slice(0,2).toUpperCase()}}</span>
        </v-avatar>

    </component>
</template>
<script>
    export default {
        props: {
            userId: Number,
            userName: {
                type: String
            },
            userImageUrl: {
                type: String,
                required: false
            }
        },
        computed: {
            isImage(){
               return  this.userImageUrl && this.userImageUrl.length > 1
            },
            strToACII() {
                let sum = 0;
                for (let i in this.userName) {
                    sum += this.userName.charCodeAt(i);
                }
                return sum
            }
        },
    }
</script>
<style src="./UserAvatar.less" lang="less"></style>
