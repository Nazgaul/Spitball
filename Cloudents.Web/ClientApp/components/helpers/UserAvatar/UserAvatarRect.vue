<template>
    <component v-if="!!userName" class="user-avatar-rect" :is="userId?'router-link':'div'" :to="userId?{name:'profile',params:{id:userId,name:userName}}:''">
        <div v-if="userImageUrl" class="user-avatar-image-wrap" :style="{width: `${width}px`, height: `${height}px`}">
            <v-progress-circular v-if="!isLoaded" indeterminate v-bind:size="50"></v-progress-circular>
            <intersection>
                <img 
                    draggable="false"
                    @load="loaded"
                    @error="onImgError"
                    :src="imageUrl"
                    :alt="userName"
                    :style="{borderRadius: `${borderRadius}px`}"
                    class="user-avatar-rect-img">
            </intersection>
        </div>
        <v-avatar v-else :tile="true" tag="v-avatar" :class="'user-avatar-rect-no-image userColor' + strToACII % 11" :style="{width: `${width}px`, height: `${height}px`, fontSize: `${fontSize}px`, borderRadius: `${borderRadius}px`}">
            <span class="white--text">{{userName.slice(0,2).toUpperCase()}}</span>
        </v-avatar>
    </component>
</template>
<script>
import utilitiesService from '../../../services/utilities/utilitiesService'; // cannot async, js error
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
        width: {
            type: Number,
            required: false
        },
        height: {
            type: Number,
            required: false
        },
        fontSize: {
            type: Number,
            required: false
        },
        borderRadius: {
            type: Number,
            required: false,
            default: 0
        }
    },
    data(){
        return{
            isLoaded: false,
            imgError: false,
        }
    },
    methods:{
        loaded() {
            this.isLoaded = true;
        },
        onImgError(){
            //this.isLoaded  = true;
            this.userImageUrl = null;
            this.imgError = true;
          
        }
    },
    computed: {
        strToACII() {
            let sum = 0;
            for (let i in this.userName) {
                sum += this.userName.charCodeAt(i);
            }
            return sum
        },
        imageUrl(){
            return utilitiesService.proccessImageURL(this.userImageUrl, this.width, this.height)
        }
    },
}
</script>

<style lang="less">
    .user-avatar-rect {
        .user-avatar-image-wrap{
            display: flex;
            justify-content: center;
            align-items: center;
            img {
                border-radius: 4px;
                width: 100%;
            }
        }
        .user-avatar-rect-no-image {
            position: unset;
            border-radius: 4px;
            font-size: 24px;
        }
    }

</style>