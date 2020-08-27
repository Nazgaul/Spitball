<template>
    <div class="courseInfo pa-5 mb-6">
        <div class="courseInfoTitle mb-9" v-t="'basic_info'"></div>
        <v-text-field 
            v-model="courseName"
            class="courseName mb-2"
            :rules="[rules.required]"
            :label="$t('dashboardPage_label_live_title')"
            placeholder=" "
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
            :rules="[rules.requiredNum, rules.minimum, rules.maximum, rules.subscriptionPrice]"
            :label="isSubscription ? $t('follower_price') : $t('price')"
            :prefix="getSymbol"
            onkeypress="return !(event.charCode == 46)"
            placeholder=" "
            dense
            color="#304FFE"
            autocomplete="off"
            height="50"
            outlined
        >
        </v-text-field>

        <v-text-field
            v-model="subscriberPrice"
            v-if="isSubscription"
            type="number"
            class="priceSubscriber mb-6"
            :rules="[rules.requiredNum, rules.minimum, rules.maximum, rules.subscriptionPrice]"
            :label="$t('subscriber_price')"
            :prefix="getSymbol"
            onkeypress="return !(event.charCode == 46)"
            placeholder=" "
            dense
            color="#304FFE"
            autocomplete="off"
            height="50"
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
            outlined
            no-resize
        ></v-textarea>

        <div class="addImage">
            <div class="addImageTitle mb-4" v-t="'add image'"></div>

            <label class="liveImageWrap d-flex flex-column">
                <uploadImage
                    v-show="isLoaded"
                    :fromLiveSession="true"
                    @setLiveImage="handleLiveImage"
                    class="editLiveImage"
                />
                <div class="noDefaultImage" v-if="!$route.params.id && !previewImage && !image">
                    <v-icon size="40" color="#bdc0d1">sbf-plus-sign</v-icon>
                </div>
                <template v-else>
                    <v-skeleton-loader v-if="!isLoaded" height="140" width="250" type="image"></v-skeleton-loader>
                    <img v-show="isLoaded" @load="loaded" class="liveImage" :src="previewImage || $proccessImageUrl(image || liveImage, 250, 140)" width="250" height="140" alt="">
                </template>
                <div class="recommendedImage mt-2" v-t="'image resolution'"></div>
            </label>
        </div>
    </div>
</template>

<script>
import { validationRules } from '../../../../services/utilities/formValidationRules.js'
import uploadImage from '../../../new_profile/profileHelpers/profileBio/bioParts/uploadImage/uploadImage.vue';

export default {
    name: 'courseInfo',
    components: {
        uploadImage
    },
    data() {
        return {
            isLoaded: false,
            previewImage: null,
            newLiveImage: null,
            rules: {
                requiredNum: (val) => (val.toString() && !isNaN(val)) || this.$t("formErrors_required"),
                required: (val) => validationRules.required(val),
                minimum: (val) => validationRules.minVal(val,0),
                maximum: (val) => validationRules.maxVal(val, 10000),
                integer: (val) => validationRules.integer(val),
                subscriptionPrice: val => {
                    if(global.country === 'IL') {
                        return val >= 5 || val <= 0 || this.$t('minimum_price')
                    }
                    return true
                }
            }
        }
    },
    computed: {
        isSubscription(){
            return this.$store.getters.getIsTutorSubscription
        },
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
                return this.$store.getters.getFollowerPrice
            },
            set(price) {
                let needPayment = this.$store.getters.getAccountNeedPayment
                if(needPayment && global.country === 'IL') {
                    this.$store.commit('setShowCourse', parseInt(price) === 0 ? true : false)
                    this.$store.commit('setComponentKey')
                }
                this.$store.commit('setFollowerPrice', price)
            }
        },
        subscriberPrice: {
            get() {
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
        image() {
            return this.$store.getters.getCourseCoverImage
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
                this.isLoaded = false
                let formData = new FormData();
                formData.append("file", previewImage[0]);
                let self = this
                this.$store.dispatch('updateLiveImage', formData).then(({data}) => {
                    self.isLoaded = false
                    self.previewImage = window.URL.createObjectURL(previewImage[0])
                    self.newLiveImage = data.fileName
                    self.$store.commit('setCourseCoverImage', self.newLiveImage)
                })
            }
        },
        loaded() {
            this.isLoaded = true
        }
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
    .priceFollower, .priceSubscriber {
        width: 180px;
    }
    .addImage {
        .addImageTitle {
            .responsive-property(font-size, 20px, null, 18px);
            font-weight: 600;
            color: @global-purple;
        }
        .liveImageWrap {
            cursor: pointer;
            max-width: fit-content;
            text-align: center;
            position: relative;
            .editLiveImage {
                position: absolute;
                text-align: center;
                border-radius: 3px;
                background-color: rgba(0,0,0,.6);
                z-index: 1;
                top: 1px;
                left: 1px;
                padding: 6px;
            }
            .liveImage {
                border: solid 1px #c6cdda;
                border-radius: 4px;
            }
            .recommendedImage {
                font-size: 16px;
                color: #adb1b4;
            }
            .noDefaultImage {
                display: flex;
                justify-content: center;
                width: 250px;
                height: 140px;
                border-radius: 6px;
                background-color: #f0f4f8
            }
        }
    }
}
</style>