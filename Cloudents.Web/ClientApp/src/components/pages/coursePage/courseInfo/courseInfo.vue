<template>
    <div class="courseInfo pa-5 mb-6">
        <div class="courseInfoTitle mb-9" v-t="'basic_info'"></div>
        <v-combobox
            v-model="courseName"
            class="courseName"
            :items="suggestsCourses"
            :rules="[rules.required]"
            @keyup="searchCourses"
            :label="$t('dashboardPage_label_live_title')"
            :placeholder="$t('course_placeholder')"
            height="50"
            color="#304FFE"
            autocomplete="off"
            dense
            outlined
        />
        <v-text-field 
            v-model="followerPrice"
            type="number"
            class="priceFollower"
            :rules="[rules.required,rules.minimum]"
            :label="$t('follower_price')"
            :prefix="getSymbol"
            placeholder=" "
            dense
            color="#304FFE"
            autocomplete="off"
            height="50"
            hide-details
            outlined
        >
        </v-text-field>

        <v-switch
            v-model="subscribeSwitch"
            class="my-6 pa-0"
            :label="$t('set_subscriber_price')"
            hide-details
        ></v-switch>

        <v-text-field
            v-model="subscriberPrice"
            v-if="subscribeSwitch"
            type="number"
            class="priceFollower mb-11"
            :rules="[rules.required,rules.minimum]"
            :label="$t('follower_price')"
            :prefix="getSymbol"
            placeholder=" "
            dense
            color="#304FFE"
            autocomplete="off"
            height="50"
            hide-details
            outlined
        >
        </v-text-field>

        <v-textarea
            v-model="courseDescription"
            class="mb-2"
            :rules="[rules.required]"
            :label="$t('description')"
            :placeholder="$t('description_placeholder')"
            :rows="7"
            color="#304FFE"
            dense
            outlined
            no-resize
        ></v-textarea>

        <div class="addImage">
            <div class="addImageTitle mb-4" v-t="'add image'"></div>

            <div class="liveImageWrap d-flex flex-column">
                <uploadImage
                    v-show="true"
                    :fromLiveSession="true"
                    @setLiveImage="handleLiveImage"
                    class="editLiveImage"
                />
                <img class="liveImage" :src="previewImage || liveImage" width="250" height="140" alt="">
                <div class="recommendedImage mt-2" v-t="'image resolution'"></div>
            </div>
        </div>
    </div>
</template>

<script>
import { validationRules } from '../../../../services/utilities/formValidationRules.js'
import uploadImage from '../../../new_profile/profileHelpers/profileBio/bioParts/uploadImage/uploadImage.vue';

import debounce from "lodash/debounce";
import courseService from '../../../../services/courseService.js';

export default {
    name: 'courseInfo',
    components: {
        uploadImage
    },
    data() {
        return {
            subscribeSwitch: false,
            suggestsCourses: [],
            previewImage: null,
            rules: {
                required: (value) => validationRules.required(value),
                minimum: (value) => validationRules.minVal(value,0),
                integer: (value) => validationRules.integer(value),
            }
        }
    },
    computed: {
        liveImage() {
            return this.isMobile ? require('../../../new_profile/components/profileLiveClasses/live-banner-mobile.png') : require('../../../new_profile/components/profileLiveClasses/live-banner-desktop.png')
        },
        courseName: {
            get() {
                return this.$store.getters.getCourseName
            },
            set(name) {
                this.$store.commit('setCourseName', name)
            }
        },
        followerPrice: {
            get() {
                //TODO: need price
                return this.$store.getters.getFollowerPrice
            },
            set(price) {
                this.$store.commit('setFollowerPrice', price)
            }
        },
        subscriberPrice: {
            get() {
                //TODO: need price
                return this.$store.getters.getSubscriberPrice
            },
            set(price) {
                this.$store.commit('setSubscriberPrice', price)
            }
        },
        courseDescription: {
            get() {
                return this.$store.getters.getDescription
            },
            set(description) {
                this.$store.commit('setCourseDescription', description)
            }
        },
        getSymbol() {
            let currencySymbol = this.$store.getters.accountUser?.currencySymbol
            let v = this.$n(1, {'style':'currency','currency': currencySymbol});
            return v.replace(/\d|[.,]/g,'').trim();
        },
    },
    methods: {
        handleLiveImage(previewImage) {
            if(previewImage) {
                let formData;
                formData = new FormData();
                let file = previewImage[0];
                formData.append("file", file);

                this.$store.dispatch('updateLiveImage', formData).then(({data}) => {
                    this.previewImage = window.URL.createObjectURL(previewImage[0])
                    this.newLiveImage = data.fileName
                })
            }
        },
        searchCourses(ev){
            let term = ev.target.value.trim()
            if(!term) {
                this.courseName = ''
                this.suggestsCourses = []
                return 
            }
            this.courseName = term;
            this.searchDebounce(term)
        },
        searchDebounce: debounce(function(term){
            courseService.getCourse({term}).then(data=>{
                this.suggestsCourses = data;
                if(this.suggestsCourses.length) {
                    this.suggestsCourses.forEach(course => {
                        if(course.text === this.courseName){
                            this.courseName = course
                        }
                    }) 
                }
            })
        }, 200)
    }
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
@import '../../../../styles/colors.less';

.courseInfo {
    background: #fff;
    border-radius: 6px;
    box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
    max-width: 760px;
    .courseInfoTitle {
        font-size: 20px;
        font-weight: 600;
        color: @global-purple;
    }
    .courseName {
        width: 500px;
    }
    .priceFollower {
        width: 140px;
    }
    .addImage {
        .addImageTitle {
            .responsive-property(font-size, 20px, null, 18px);
            font-weight: 600;
            color: @global-purple;
        }
        .liveImageWrap {
            max-width: fit-content;
            text-align: center;
            position: relative;
            .editLiveImage {
                position: absolute;
                text-align: center;
                border-radius: 3px;
                background-color: rgba(0,0,0,.6);
                z-index: 1;
                left: 0;
            }
            .liveImage {
                border: solid 1px #c6cdda;
                border-radius: 4px;
            }
            .recommendedImage {
                font-size: 16px;
                color: #adb1b4;
            }
        }
    }
}
</style>