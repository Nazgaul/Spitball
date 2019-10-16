<template>
    <div class="become-first-wrap" :class="[$vuetify.breakpoint.smAndUp ? 'px-0' : '']">
        <span class="become-first-span" v-language:inner="'becomeTutor_sharing_step_1'"></span>
        <v-layout row wrap align-start class="become-first-cont">
            <v-flex xs12 sm4 shrink class="image-wrap text-xs-center">
                <img v-show="userImage && isLoaded" class="user-image" :src="userImage" alt="upload image" @load="loaded">
                <div v-if="!isLoaded" class="image-loader">
                    <v-progress-circular indeterminate :size="isMobile? 70: 180" width="3" color="info"></v-progress-circular>
                </div>

                <label for="tutor-picture" v-if="!userImage" class="font-weight-bold" :class="[errorUpload ?  'error-upload': '']">
                    <img v-show="!userImage && isLoaded" class="user-no-image" 
                         src="../images/group-copy-2.png" alt="upload image"
                         @load="loaded">
                    <input class="become-upload"
                        type="file" name="File Upload"
                        @change="uploadImage"
                        id="tutor-picture"
                        accept="image/*"
                        ref="tutorImage" v-show="false"/>
                    <div v-if="errorUpload" v-language:inner="'becomeTutor_upload_error'"></div>
                    <!-- <span class="image-edit-text" v-language:inner="'becomeTutor_upload_image'"></span> -->
                </label>
            </v-flex>
            <v-flex xs12 sm6 class="inputs-wrap" :class="{'mt-2' : $vuetify.breakpoint.xsOnly}">
                <v-layout column shrink justify-start>
                    <v-form v-model="validBecomeFirst" ref="becomeFormFirst">
                        <v-flex xs12 shrink class="mb-2">
                            <v-text-field
                            autocomplete="abcd"
                            v-model="firstName"
                            :rules="[rules.required, rules.notSpaces, rules.minimumChars]"
                            class="become-tutor-edit-firstname"
                            :placeholder="placeFirstName" 
                            :label="placeFirstName"/>
                        </v-flex>
                        <v-flex xs12 class="mb-2">
                            <v-text-field
                                v-model="lastName"
                                :rules="[rules.required, rules.notSpaces, rules.minimumChars]"
                                class="become-tutor-edit-lastname"
                                :placeholder="placeLastName" 
                                :label="placeLastName"/>
                        </v-flex>
                        <v-flex xs12 class="mt-2 first-selects">
                            <v-text-field 
                                class="font-weight-bold price-input"
                                :rules="[rules.required, rules.minimum, rules.maximum,rules.integer]"
                                v-model="price"
                                type="number"
                                :label="placePrice"/>

                            <!-- <v-select
                                v-model="gender"
                                :items="genderItems"
                                class="font-weight-bold price-input"
                                :rules="[rules.required]"
                                :label="selectGender"
                                :append-icon="'sbf-arrow-down'">
                            </v-select> -->
                        </v-flex>
                    </v-form>
                </v-layout>
            </v-flex>
        </v-layout>
        <v-layout class="mt-4 px-1 btns-first"
                  :class="[$vuetify.breakpoint.smAndUp ? 'align-end justify-end' : 'align-center justify-center']">

            <v-btn @click="closeDialog()" class="cancel-btn elevation-0" round outline flat>
                <span v-language:inner>becomeTutor_btn_cancel</span>
            </v-btn>

            <v-btn
                    color="#4452FC"
                    round
                    class="white-text elevation-0 btn-first_next-btn"
                    :disabled="btnDisabled"
                    @click="nextStep()">
                <span v-language:inner>becomeTutor_btn_next</span>
            </v-btn>

        </v-layout>
    </div>
</template>

<script>
    import { mapActions, mapGetters } from 'vuex';
    import utilitiesService from '../../../services/utilities/utilitiesService';
    import { validationRules } from '../../../services/utilities/formValidationRules';
    import { LanguageService } from "../../../services/language/languageService";

    export default {
        name: "firstStep",
        data() {
            return {
                placeFirstName: LanguageService.getValueByKey("becomeTutor_placeholder_first_name"),
                placeLastName: LanguageService.getValueByKey("becomeTutor_placeholder_last_name"),
                placePrice: LanguageService.getValueByKey("becomeTutor_placeholder_price"),
                selectGender: LanguageService.getValueByKey("becomeTutor_placeholder_select_gender"),
                firstName: '',
                lastName: '',
                price: 50,
                imageAdded: false,
                errorUpload: false,
                validBecomeFirst: false,
                rules: {
                    required: (value) => validationRules.required(value),
                    minimum: (value) => validationRules.minVal(value,50),
                    maximum: (value) => validationRules.maxVal(value, 1000),
                    minimumChars: (value) => validationRules.minimumChars(value, 2),
                    notSpaces: (value) => validationRules.notSpaces(value),
                    integer: (value) => validationRules.integer(value)
                },
                isLoaded: false,
                gender: LanguageService.getValueByKey("becomeTutor_gender_male"),
                genderItems:[LanguageService.getValueByKey("becomeTutor_gender_male"),LanguageService.getValueByKey("becomeTutor_gender_female")]
            };
        },
        computed: {
            ...mapGetters(['becomeTutorData', 'accountUser']),
            btnDisabled() {
                return false
                // return !this.firstName || !this.lastName || !this.price || !this.imageExists;
            },
            userImage() {
                let mobile = this.$vuetify.breakpoint.xsOnly;
                let size = mobile ? [80, 90] : [190, 210];
                if(this.accountUser && this.accountUser.image) {
                    return utilitiesService.proccessImageURL(this.accountUser.image, ...size);
                }
                return '';
            },
            isMobile(){
                return this.$vuetify.breakpoint.xsOnly;
            }
        },
        methods: {
            ...mapActions(['updateTutorInfo', 'uploadAccountImage', 'updateTutorDialog', 'updateToasterParams']),
            loaded() {
                this.isLoaded = true;
            },
            uploadImage() {
                let self = this;
                let formData = new FormData();
                let file = self.$refs.tutorImage.files[0];
                this.isLoaded = false;
                this.errorUpload = false;
                formData.append("file", file);
                self.uploadAccountImage(formData).then((done) => {
                    if(!done) {
                        this.updateToasterParams({
                            toasterText: LanguageService.getValueByKey("chat_file_error"),
                            showToaster: true
                        });
                        return;
                    }
                    self.imageAdded = true;
                    }).catch((error) => {
                        self.imageAdded = false;
                        self.errorUpload = true;
                    });
            },
            nextStep() {
                if(!this.imageAdded && !this.userImage ){
                    this.errorUpload = true;
                    this.$refs.becomeFormFirst.validate();
                    return
                }
                if(this.$refs.becomeFormFirst.validate()) {
                    
                    let data = {
                        image: this.userImage,
                        firstName: this.firstName,
                        lastName: this.lastName,
                        price: this.price
                    };
                    this.updateTutorInfo(data);
                    this.$root.$emit('becomeTutorStep', 2);
                }
            },
            closeDialog() {
                this.updateTutorDialog(false);
            }
        },
    };
</script>

<style lang="less">
    @import '../../../styles/mixin.less';

    .become-first-wrap {
        @media (max-width: @screen-xs) {
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: space-between;
            height: 100%;
        }
        .btns-first{
            @media (max-width: @screen-xs) {
                align-items: flex-end;
            }
            .v-btn {
              @media (max-width: @screen-xs) {
                  height: 40px;
                  padding: 0 20px;
                  text-transform: capitalize;
              }  
            }
            .btn-first_next-btn {
                padding: 0 26px;
            }
        }

        
        .become-first-span{
            padding-left: 30px;
            font-size: 16px;        
            letter-spacing: -0.51px;
            color: @global-purple;
            @media (max-width: @screen-xs) {
                padding-left: 0;
                text-align: center;
                font-size: 16px;
                margin-bottom: 10px;
                font-weight: 600;
                line-height: 1.5;
                letter-spacing: 0.3px;
            }
        }
        .become-first-cont{
            padding-left: 30px;
            padding-top: 30px;
            @media (max-width: @screen-xs) {
                padding-left: 0;
                align-items: center;
                width: 100%;
                padding-top: 8px;
            }
            .first-selects{
                display: flex;
                justify-content: space-between;
            }
        }
        .error-upload{
            color: red!important;
        }
        .contact-price {
            margin-top: 20px;
        }
        .image-wrap {
            display: flex;
            position: relative;
            min-width: 220px;
            max-width: 220px;
            @media (max-width: @screen-xs) {
                padding-top: 6px;
                padding-bottom: 12px;
                margin: 0 auto;
                justify-content: center;
            }
            .image-loader {
                width: 100%;
                height: 200px;
                display: flex;
                justify-content: center;
                align-items: center;
                @media (max-width: @screen-xs) {
                    height: auto;
                }
            }
        }
        .price-input {
            color: @textColor;
            width: 40%;
                max-width: 40%;
            @media (max-width: @screen-xs) {
                width: 100%;
            }
        }
        .inputs-wrap {
            margin-left: 10px;
            min-width: 60%;
            @media (max-width: @screen-xs) {
                width: 100%;
                margin-left: unset;
            }
            .become-tutor-edit-firstname, .become-tutor-edit-lastname {
                .v-messages__message {
                    line-height: normal;
                }
            }
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
            color: @global-purple;
            padding: 12px 18px;
            cursor: pointer;
        }
        .user-image {
            border-radius: 6px;
            border: 1px solid #f0f0f7;
        }
        .user-no-image {
            cursor: pointer;
            // max-width: 190px;
            // min-height: 210px;
                object-fit: cover;
            @media (max-width: @screen-xs) {
            // max-width: 80px;
            // min-height: 80px;
            height: 90px;
            width: 80px;
            }
            border-radius: 6px;
            border: 1px solid #f0f0f7;
        }
        .blue-text {
            color: @global-blue;
        }
        .v-input__slot .v-text-field__slot label {
            color: @global-purple;
            font-size: 16px;
        }
        .v-input{
            input{
                // height: 50px;
                // max-height: 50px;
            @media (max-width: @screen-xs) {
                max-height: 44px;
            }

            }
        } 
        .v-text-field{
            input{
                font-size: 16px;
            }
        }
        .v-text-field--outline > .v-input__control > .v-input__slot {
            border: solid 1px rgba(0, 0, 0, 0.19);
            &:hover {
                border: 1px solid rgba(0, 0, 0, 0.19) !important;
            }
        }
    }
</style>