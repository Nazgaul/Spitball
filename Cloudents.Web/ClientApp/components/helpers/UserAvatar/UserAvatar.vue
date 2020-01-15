<template>
    <component v-if="!!userName" :is="userId?'router-link':'div'" :to="userId?{name:'profile',params:{id:userId,name:userName}}:''">
        <v-lazy
            v-if="isImage"
            v-model="isActive"
            :options="{
                threshold: .5
            }"
            transition="fade-transition"
        >
            <v-avatar tag="v-avatar" :size="size" :class="'user-avatar image'">
                <img @error="onImgError" :src="imageUrl" alt="user avatar" class="user-avatar-img">
            </v-avatar>
        </v-lazy>
        <v-avatar v-else tag="v-avatar" :size="size" :class="'user-avatar userColor' + strToACII % 11">
            <span class="white--text font-14">{{userName.slice(0,2).toUpperCase()}}</span>
        </v-avatar>

    </component>
</template>
<script>
import utilitiesService from '../../../services/utilities/utilitiesService';
    export default {
        props: {
            userId: Number,
            userName: {
                type: String
            },
            userImageUrl: {
                type: String,
                required: false
            },
            size:{
                type:String,
                required: false,
                default: '32'
            }
        },
        data(){
            return{
                imgError: false,
                isActive: false,
            }
        },
        methods:{
            onImgError(){
                this.imgError = true;
            }
        },
        computed: {
            isImage(){
               return  this.userImageUrl && this.userImageUrl.length > 1 && !this.imgError
            },
            strToACII() {
                let sum = 0;
                for (let i in this.userName) {
                    sum += this.userName.charCodeAt(i);
                }
                return sum
            },
            imageUrl(){
                let size = this.size ? this.size : 32;
                return utilitiesService.proccessImageURL(this.userImageUrl, size, size)
            }
        },
    }
</script>
<style src="./UserAvatar.less" lang="less"></style>
