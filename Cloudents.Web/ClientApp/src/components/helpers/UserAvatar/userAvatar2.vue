<template>
    <component
        v-if="!!userName"
        class="user-avatar-rect"
        :is="userId ? 'router-link' : 'div'"
        :to="userId ? {name: 'profile', params: { id: userId, name: userName }} : ''"
    >
        <div v-if="userImageUrl" class="user-avatar-image-wrap" :style="{width: `${width}px`}">
            <v-skeleton-loader
                v-if="!loading && !isLoaded"
                class="skeletonAvatar"
                :type="tile ? 'image' : 'avatar'"
                :width="width"
                :height="height"
                :style="{borderRadius: tile ? `${borderRadius}px` : `50%`}"
            >
            </v-skeleton-loader>
            <intersection>
                <img
                    v-show="isLoaded"
                    draggable="false"
                    @load="loaded"
                    @error="onImgError"
                    :src="imageUrl"
                    :alt="userName"
                    :style="{borderRadius: tile ? `${borderRadius}px` : `50%`}"
                    class="user-avatar-rect-img"
                />
            </intersection>
        </div>
        <v-avatar
            v-else
            :tile="tile"
            tag="v-avatar"
            class="user-avatar-rect-no-image"
            :class="`userColor${strToACII % 11}`"
            :width="width"
            :min-width="width"
            :height="height"
            :min-height="height"
            :style="{fontSize: `${fontSize}px`, borderRadius: tile ? `${borderRadius}px` : `50%`}"
        >
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
        tile: {
            type: Boolean,
            required: false
        },
        width: {
            type: Number,
            required: true
        },
        height: {
            type: Number,
            required: true
        },
        fontSize: {
            type: Number,
            required: true
        },
        borderRadius: {
            type: Number,
            required: false,
            default: 0
        },
        loading: {
            type: Boolean,
            required: false
        }
    },
    data(){
        return{
            isLoaded: false,
            imgError: false,
        }
    },
    watch: {
        loading(val) {
            if(!val) {
                this.isLoaded = false
            }
        }
    },
    methods:{
        loaded() {
            this.isLoaded = true;
            this.$emit('setAvatarLoaded', true)
        },
        onImgError(){
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
                width: 100%;
            }
            .skeletonAvatar {
                .v-skeleton-loader__image, .v-skeleton-loader__avatar {
                    height: inherit;
                    width: inherit;
                    border-radius: inherit;
                }
            }
        }
    }

</style>