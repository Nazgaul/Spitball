<template>
    <router-link :to="{name: 'profile', params: {id: tutorData.userId,name:tutorData.name}}">
        <div class="studentCard pa-3">
            <div class="caption pb-3" v-language:inner="'documentPage_student_uploaded'"></div>
            <div class="studentCard-details align-center">
                <img class="tutor-image mr-2" v-show="isLoaded" v-if="!userImageUrl" @error="onImageLoadError" @load="loaded" :src="userImageUrl" :alt="tutorData.name">
                <img class="tutor-image mr-2 default-img" v-show="isLoaded" v-else @error="onImageLoadError" @load="loaded" :src="userImageUrl" :alt="tutorData.name">
                <div>
                    <div class="studentCard_box align-center">
                        <div class="studentCard_name body-2 font-weight-bold text-truncate px-2">{{tutorData.name}}</div>
                        <user-rank style="margin: 15px auto;" :score="tutorData.score"></user-rank>
                    </div>
                    <router-link :to="{name: 'profile', params: {id: tutorData.userId, name:tutorData.name}}"><div class="pt-3" v-language:inner="'documentPage_student_views_documents'"></div></router-link>
                </div>
            </div>
        </div>
    </router-link>
</template>
<script>
import utilitiesService from "../../services/utilities/utilitiesService";
import userRank from "../helpers/UserRank/UserRank.vue";

export default {
    components: {
        userRank
    },
    props: {
        tutorData:{}
    },
    data() {
        return {
            isLoaded: false,
        }
    },
    methods: {
        loaded() {
            this.isLoaded = true;
        },
        onImageLoadError(event) {            
            event.target.src = "./images/placeholder-profile.png";
        }
    },
    computed: {
        userImageUrl() {
            if (this.tutorData.image) {
                let size = [76, 96];
                return utilitiesService.proccessImageURL(
                this.tutorData.image,
                ...size,
                "crop"
                );
            } else {
                return "./images/placeholder-profile.png";
            }
        },
    },
}
</script>

<style lang="less">
    .studentCard {
        background: #fff;
        border-radius: 4px;
        .caption {
            color: #43425d;
        }
        .studentCard-details {
            display: flex;
            .default-img {
                height: 96px;
                width: 76px;
            }
            .studentCard_box {
                display: flex;
                .studentCard_name {
                    max-width: 130px;
                }
                .studentCard_level {
                    flex-direction: column;
                    padding: 0 10px;
                    border-radius: 14px;
                    background-color: #43425d;
                    max-width: 130px;
                }
            }
        }
    }
</style>