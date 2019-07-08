<template>
    <router-link @click.native.prevent="tutorCardClicked" :to="{name: 'profile', params: {id: tutorData.userId,name:tutorData.name}}">
        <div class="studentCard pa-3">
            <div class="caption pb-3" v-language:inner="'documentPage_student_uploaded'"></div>
            <div class="studentCard-details align-center">
                <img class="tutor-image mr-2" v-show="isLoaded" @error="onImageLoadError" @load="loaded" :src="userImageUrl" :alt="tutorData.name">
                <div>
                    <div class="studentCard_box">
                        <div class="studentCard_name body-2 font-weight-bold text-truncate">{{tutorData.name}}</div>
                        <span class="studentCard_level white--text caption ml-3" v-language:inner="'documentPage_student_level'"></span>
                    </div>
                    <router-link :to="{name: 'profile', params: {id: tutorData.userId, name:tutorData.name}}"><div class="pt-3" v-language:inner="'documentPage_student_views_documents'"></div></router-link>
                </div>
            </div>
        </div>
    </router-link>
</template>
<script>
import utilitiesService from "../../services/utilities/utilitiesService";

export default {
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
    }
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