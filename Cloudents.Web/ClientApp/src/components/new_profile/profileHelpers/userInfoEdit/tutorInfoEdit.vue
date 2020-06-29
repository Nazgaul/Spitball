<template>
    <v-dialog :value="true" :content-class="'profileTutorEditInfo'" persistent max-width="710px" :fullscreen="$vuetify.breakpoint.xsOnly">
        <v-form v-model="valid" ref="formTutor" class="pa-4">
            <div class="header d-flex justify-space-between align-start mb-8">
                <div class="mainTitle" v-t="'upload images'"></div>
                <v-icon class="closeIcon" size="12" @click="closeDialog">{{$vuetify.icons.values.close}}</v-icon>
            </div>

            <div class="profilePicture mb-5">
                <div class="pictureTitle text-center mb-2" v-t="'profile picture'"></div>
                <uploadImage sel="photo" class="editImage" />
                <userAvatarNew
                    sel="avatar_image"
                    class="pUb_dS_img"
                    :userName="$store.getters.getAccountName"
                    :userImageUrl="$store.getters.getAccountImage"
                    :width="isMobile ? 130: 160"
                    :height="isMobile ? 161 : 200"
                    :userId="$store.getters.getAccountId"
                    :fontSize="36"
                    :borderRadius="8"
                    :tile="true"
                />
            </div>

            <div class="profileCover mb-12">
                <div class="coverTitle mb-2" v-t="'profile cover'"></div>
                <cover />
            </div>

            <v-row no-gutters class="profileInfo">
                <v-col cols="12" class="mb-4">
                    <div class="infoTitle" v-t="'profile_personal_details'"></div>
                </v-col>
                <v-col cols="12" sm="6">
                    <v-text-field
                        class="me-sm-5"
                        :rules="[rules.required, rules.minimumChars]"
                        :label="$t('profile_firstName_label')"
                        v-model.trim="firstName"
                        outlined
                    ></v-text-field>
                </v-col>
                <v-col cols="12" sm="6">
                    <v-text-field
                        :rules="[rules.required, rules.minimumChars]"
                        :label="$t('profile_firstName_label')"
                        v-model.trim="lastName"
                        outlined
                    ></v-text-field>
                </v-col>
                <v-col cols="12">
                    <v-textarea
                        rows="2"
                        outlined
                        v-model="title"
                        :rules="[rules.minimum, rules.descriptionMaxChars]"
                        counter="28"
                        :label="$t('profile_description_label')"
                    ></v-textarea>
                </v-col>
                <v-col cols="12">
                    <v-textarea
                        rows="2"
                        outlined
                        v-model="shortParagraph"
                        :rules="[rules.minimum, rules.shortParagraphMaxChars]"
                        counter="96"
                        :label="$t('Short paragraph')"
                        :placeholder="$t('shortParagraph placeholder')"
                    ></v-textarea>
                </v-col>
                <v-col>
                    <v-textarea
                        rows="5"
                        outlined
                        :rules="[rules.minimum, rules.maximumChars]"
                        v-model="bio"
                        class="tutor-edit-bio"
                        :label="$t('profile_bio_label')"
                        :placeholder="$t('bio placeholder')"
                    ></v-textarea>
                </v-col>
            </v-row>

            <div class="text-center">
                <v-btn :disabled="btnLoading" width="120" depressed color="#4452fc" class="shallow-blue ml-0" rounded outlined primary @click="closeDialog">
                    <span v-t="'cancel'"></span>
                </v-btn>
                <v-btn class="blue-btn white--text ms-4" width="120" depressed color="#4452fc" rounded @click="saveChanges" :loading="btnLoading">
                    <span v-t="'save'"></span>
                </v-btn>
            </div>
        </v-form>
    </v-dialog>
</template>

<script>
import { TUTOR_EDIT_PROFILE } from '../../../pages/global/toasterInjection/componentConsts'
import { validationRules } from '../../../../services/utilities/formValidationRules';
import uploadImage from '../../profileHelpers/profileBio/bioParts/uploadImage/uploadImage.vue'
import cover from '../../components/cover.vue'

export default {
    name: "tutorInfoEdit",
    components: {
        uploadImage,
        cover
    },
    data() {
        return {
            editedBio: '',
            editTitle: '',
            editShortParagraph: '',
            editedFirstName: '',
            editedLastName: '',
            valid: false,
            btnLoading: false,
            rules: {
                required: (value) => validationRules.required(value),
                minimum: (value) => validationRules.minVal(value.length, 10),
                maximum: (value) => validationRules.maxVal(value, 1000),
                maximumChars: (value) => validationRules.maximumChars(value, 1000),
                minimumChars: (value) => validationRules.minimumChars(value, 2),

                descriptionMaxChars: (value) => validationRules.maximumChars(value, 28),
                shortParagraphMaxChars: (value) => validationRules.maximumChars(value, 96)
            }
        };
    },
    computed: {
        coverImage() {
            return this.$store.getters.getProfileCoverImage
        },
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly
        },
        shortParagraph: {
            get() {
                return this.$store.getters.getProfileBio
            },
            set(newVal) {
                this.editShortParagraph = newVal;
            }
        },
        bio: {
            get() {
                return this.$store.getters.getProfileParagraph
            },
            set(newVal) {
                this.editedBio = newVal;
            }
        },
        firstName: {
            get() {
                return this.$store.getters.getProfileFirstName;
            },
            set(newVal) {
                this.editedFirstName = newVal;
            }
        },
        lastName: {
            get() {
                return this.$store.getters.getProfileLastName;
            },
            set(newVal) {
                this.editedLastName = newVal;
            }
        },
        title: {
            get() {
                return this.$store.getters.getProfileTitle;
            },
            set(newVal) {
                this.editTitle = newVal;
            }
        }
    },
    methods: {
        saveChanges() {
            if(this.$refs.formTutor.validate()) {
                this.btnLoading = true;
                let serverFormat = {
                    firstName: this.editedFirstName || this.firstName,
                    lastName: this.editedLastName || this.lastName,
                    shortParagraph: this.editShortParagraph || this.shortParagraph,
                    bio: this.editedBio || this.bio,
                    title: this.editTitle || this.title
                };
                this.$store.dispatch('saveUserInfo', serverFormat)
                    .then(() => {
                        this.closeDialog();
                    }, (error) => {
                        console.error(error);
                    }).finally(() => {
                        this.btnLoading = false;
                    });
            }
        },
        closeDialog() {
            this.$store.commit('removeComponent', TUTOR_EDIT_PROFILE)
            // this.$store.commit('setEditDialog', false);
        }
    }
};
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';

    .profileTutorEditInfo {
        background: #fff;
        @media (max-width: @screen-xs) {
            overflow-x: hidden;
        }
        .header {
            .mainTitle {
                .responsive-property(font-size, 22px, null, 20px);
                font-weight: 600;
                color: #43425d;
            }
        }
        .profilePicture {
            position: relative;
            width: max-content;
            margin: 0 auto;
            @media (max-width: @screen-xs) {
                padding: 8px 6px;
                background: #fff;
                border-radius: 8px;
            }
            .pictureTitle {
                .responsive-property(font-size, 18px, null, 16px);
                font-weight: 600;
                color: #131415;
            }
            .pUb_dS_img{
                pointer-events: none !important;
            }
            .editImage{
                position: absolute;
                // right: 4px;
                text-align: center;
                // width: 36px;
                // height: 46px;
                border-radius: 3px;
                background-color: rgba(0,0,0,.6);
                z-index: 1;
            }
            .user-avatar-image-wrap {
                width: auto !important;
                .user-avatar-rect-img {
                    border-radius: 3px !important;
                }
            }
        }
        .profileCover {
            position: relative;
            .coverTitle {
                .responsive-property(font-size, 22px, null, 16px);
                font-weight: 600;
                color: #43425d;
            }
            .coverPhoto {
                height: 212px;
            }
        }
        .profileInfo {
            .infoTitle {
                .responsive-property(font-size, 22px, null, 20px);
                font-weight: 600;
                color: #43425d;
            }
            textarea::placeholder {
                color: #a4a7ab;
                font-size: 14px;
            }
            .v-input__slot {
                fieldset {
                    border: 1px solid #b8c0d1;
                }
                .v-label {
                    color: @global-purple;
                }
            }
        }
        .shallow-blue {
            border: 1px solid @global-blue;
            color: @color-blue-new;
            @media (max-width: @screen-xs) {
                padding: 0 16px ;
            }
        }
        // .blue-btn {
        //     @media (max-width: @screen-xs) {
        //         padding: 0 16px ;
        //     }
        // }
    }


</style>