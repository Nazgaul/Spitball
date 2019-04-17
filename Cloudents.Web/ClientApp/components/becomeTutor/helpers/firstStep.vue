<template>
    <div class="become-first-wrap" :class="[$vuetify.breakpoint.smAndUp ? 'px-0' : '']">
        <v-layout row wrap align-start  justify-center >
            <v-flex xs12 sm4 md4 shrink class="image-wrap text-xs-center">
                <img v-show="userImage && isLoaded" class="user-image" :src="userImage" alt="upload image" @load="loaded">
                <img v-show="!userImage" class="user-image" src="../images/placeholder-image.png" alt="upload image" @load="loaded" >
                <div  v-if="!isLoaded">
                    <v-progress-circular indeterminate v-bind:size="50" color="amber"></v-progress-circular>
                </div>
                <button v-show="!userImage" class="upload-btn font-weight-bold">
                    <span class="font-weight-bold"></span>
                    <input class="become-upload"
                           type="file" name="File Upload"
                           @change="uploadImage"
                           id="tutor-picture"
                           accept="image/*"
                           ref="tutorImage" v-show="false"/>
                    <label for="tutor-picture">
                        <span class="image-edit-text">Upload Your Photo</span>
                    </label>
                </button>


            </v-flex>
            <v-flex xs12 sm6 md6 class="inputs-wrap" :class="{'mt-3' : $vuetify.breakpoint.xsOnly}">
                <v-layout column shrink  justify-start>
                    <v-flex xs12  shrink :class="[$vuetify.breakpoint.smAndUp ? 'mb-3' : 'mb-3']">
                        <v-text-field outline
                                      v-model="firstName"
                                      placeholder="First Name"
                                      hide-details
                                      :label="'First Name'"></v-text-field>
                    </v-flex>
                    <v-flex xs12 :class="[$vuetify.breakpoint.smAndUp ? 'mb-4' : 'mb-3']">
                        <v-text-field outline
                                      v-model="lastName"
                                      placeholder="Last Name"
                                      hide-details
                                      :label="'Last Name'"></v-text-field>

                    </v-flex>
                    <v-flex xs12 class="mt-2">
                        <v-text-field outline class="bg-greyed font-weight-bold price-input"
                                      v-model="price"
                                      prefix="₪"
                                      hide-details
                                      :label="'Fixed Price per hour'"></v-text-field>
                    </v-flex>
                    <v-flex xs12 class="contact-price">
                        <div class="blue-text caption cursor-pointer">Contact us to change price</div>
                    </v-flex>
                </v-layout>
            </v-flex>
        </v-layout>
        <v-layout  class="mt-5 px-1" :class="[$vuetify.breakpoint.smAndUp ? 'align-end justify-end' : 'align-center justify-center']">
            <v-btn   @click="closeDialog()" class="cancel-btn elevation-0" round outline flat>
                <span>Cancel</span>
            </v-btn>
            <v-btn
                   color="#4452FC"
                   round
                   class="white-text elevation-0"
                   :disabled="btnDisabled"
                   @click="nextStep()">
                <span>Next</span>
            </v-btn>

        </v-layout>
    </div>
</template>

<script>
    import { mapActions, mapGetters } from 'vuex';
    import utilitiesService from '../../../services/utilities/utilitiesService';

    export default {
        name: "firstStep",
        data() {
            return {
                firstName: '',
                lastName: '',
                price: '50',
                isLoaded: false
            };
        },
        computed: {
            ...mapGetters(['becomeTutorData', 'accountUser']),
            btnDisabled(){
                return !this.firstName || !this.lastName
            },
            userImage() {
                if(this.accountUser && this.accountUser.image) {
                    return utilitiesService.proccessImageURL(this.accountUser.image, 240, 214, 'crop');
                } else {
                    return '';
                }
            },

        },
        methods: {
            ...mapActions(['updateTutorInfo', 'uploadAccountImage', 'updateTutorDialog']),
            loaded(){
                this.isLoaded = true;
            },
            uploadImage() {
                let self = this;
                let formData = new FormData();
                let file = self.$refs.tutorImage.files[0];
                formData.append("file", file);
                self.uploadAccountImage(formData);

            },
            nextStep(){
                let data = { image: this.userImage, firstName: this.firstName, lastName: this.lastName, price: this.price };
                this.updateTutorInfo(data);
                this.$root.$emit('becomeTutorStep', 2);
            },
            closeDialog(){
                this.updateTutorDialog(false)
            }
        },
    };
</script>

<style lang="less">
    @import '../../../styles/mixin.less';

    .become-first-wrap {
        .contact-price{
            margin-top: 20px;
        }
        .image-wrap {
            position: relative;
            min-width: 214px;
            max-width: 214px;
        }
        .bg-greyed{
            background-color: #f5f5f5;
        }
        .price-input{
            color: @textColor;
        }
        .inputs-wrap{
            margin-left: 20px;
            @media(max-width: @screen-xs){
                margin-left: unset;
            }
        }
        label[for=tutor-picture] {
            width: 162px;
            height: 46px;
        }
        .upload-btn {
            position: absolute;
            bottom: 26px;
            /*keep this to center*/
            left: 0;
            right: 0;
            margin: 0 auto;
            width: 164px;
            border-radius: 4px;
            border: solid 1px rgba(67, 66, 93, 0.56);
            font-size: 12px;
            font-weight: bold;
            color: @profileTextColor;
            padding: 12px 18px;
        }
        .user-image {
            min-width: 214px;
            max-width: 214px;
            min-height: 240px;
            border-radius: 4px;
        }
        .blue-text {
            color: @colorBlue;
        }
        .v-input__slot .v-text-field__slot label {
            color: @profileTextColor;
        }
        .v-text-field--outline > .v-input__control > .v-input__slot {
            border: solid 1px rgba(0, 0, 0, 0.19);
            &:hover {
                border: 1px solid rgba(0, 0, 0, 0.19) !important;
            }
        }
    }
</style>