<template>
    <component v-if="!!userName" :is="userId?'router-link':'div'" :to="userId?{name:'profile',params:{id:userId,name:userName}}:''">
        <div v-if="isImage" class="imageWrapper" :style="{width: `${size}px`, height: `${size}px`}">
            <v-skeleton-loader v-if="!isLoaded" type="avatar"></v-skeleton-loader>
            <intersection>
                <v-avatar tag="v-avatar" :size="size" :class="'user-avatar image'">
                    <v-img @error="onImgError" @load="loaded" :src="imageUrl" alt="user avatar" class="user-avatar-img"></v-img>
                </v-avatar>
            </intersection>
        </div>
        <v-avatar v-else tag="v-avatar" :size="size" :class="'user-avatar userColor' + strToACII % 11">
            <span class="white--text font-14">{{userName.slice(0,2).toUpperCase()}}</span>
        </v-avatar>
    </component>
</template>
<script>
import utilitiesService from '../../../services/utilities/utilitiesService';
const intersection = () => import('../../pages/global/intersection/intersection.vue');

    export default {
        components: {intersection},
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
                isLoaded: false,
            }
        },
        methods:{
            onImgError(){
                this.imgError = true;
            },
            loaded() {
                this.isLoaded = true;
            },
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
