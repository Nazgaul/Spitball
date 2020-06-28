<template>
    <v-card class="tutor-edit-wrap pb-4">
        <v-form v-model="valid" ref="formTutor">
            <v-layout class="header pa-4 mb-4">
                <v-flex>
                    <v-icon class="edit-icon mr-2">sbf-edit-icon</v-icon>
                    <span v-t="'profile_edit_user_profile_title'"></span>
                </v-flex>
            </v-layout>
            <v-layout class="px-3 mt-4">
                <div class="leftSide me-3 d-inline-block">
                    <uploadImage sel="photo" class="pUb_edit_img" />
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
                <v-flex xs12 :class="{'pr-2': $vuetify.breakpoint.smAndUp}">
                    <v-flex xs12 sm6 class="pl-2 mb-2">
                        <span class="subtitle" v-t="'profile_personal_details'"></span>
                    </v-flex>
                    <v-layout column>
                        <v-flex xs12>
                            <v-text-field
                                    :rules="[rules.required, rules.minimumChars]"
                                    :label="$t('profile_firstName_label')"
                                    class="tutor-edit-firstname"
                                    v-model.trim="firstName"
                                    outlined
                            ></v-text-field>
                        </v-flex>
                    </v-layout>
                <!-- </v-flex>
                <v-flex xs12 :class="[ $vuetify.breakpoint.xsOnly ? 'mt-2 mr-0' : 'pr-2']"> -->
                    <!-- <v-layout column> -->
                        <!-- <v-flex v-if="$vuetify.breakpoint.smAndUp" xs12 sm6  class="mb-2 pl-2" grow>
                            <span class="subtitle" style="visibility: hidden">hidden</span>
                        </v-flex> -->
                        <v-flex>
                            <v-text-field
                                    :rules="[rules.required, rules.minimumChars]"
                                    :label="$t('profile_lastName_label')"
                                    class="tutor-edit-lastname"
                                    v-model.trim="lastName"
                                    outlined
                            ></v-text-field>
                        </v-flex>
                    <!-- </v-layout> -->
                </v-flex>
            </v-layout>

            <v-layout class="px-3 mt-4 mt-sm-0" column>
                <v-flex class="mb-2 pl-2">
                    <span class="subtitle" v-t="'profile_aboutme'"></span>
                </v-flex>
                <v-flex>
                    <v-textarea
                        rows="2"
                        outlined
                        v-model="title"
                        :rules="[rules.descriptionMaxChars]"
                        counter="28"
                        name="input-about"
                        :label="$t('profile_description_label')"
                    ></v-textarea>
                </v-flex>
            </v-layout>
            <v-layout class="px-3 mt-2 mt-sm-0" column>
                <v-flex>
                    <v-textarea
                        rows="2"
                        outlined
                        v-model="shortParagraph"
                        :rules="[rules.shortParagraphMaxChars]"
                        counter="96"
                        name="input-about"
                        :label="$t('Short paragraph')"
                    ></v-textarea>
                </v-flex>
            </v-layout>
            <v-layout class="px-3">
                <v-flex>
                    <v-textarea
                        rows="5"
                        outlined
                        :rules="[rules.maximumChars]"
                        v-model="bio"
                        class="tutor-edit-bio"
                        name="input-bio"
                        :label="$t('profile_bio_label')"
                    ></v-textarea>
                </v-flex>
            </v-layout>
            <v-layout align-center class="px-3" :class="[$vuetify.breakpoint.xsOnly ? 'justify-space-between' : 'justify-end']">
                <v-btn :disabled="btnLoading" width="120" depressed color="#4452fc" class="shallow-blue ml-0" rounded outlined primary @click="closeDialog">
                    <span v-t="'profile_btn_cancel'"></span>
                </v-btn>
                <v-btn class="blue-btn ml-sm-4" width="120" depressed color="#4452fc" rounded @click="saveChanges()" :loading="btnLoading">
                    <span v-t="'profile_btn_save_changes'"></span>
                </v-btn>
            </v-layout>
        </v-form>
    </v-card>
</template>

<script>
import { validationRules } from '../../../../services/utilities/formValidationRules';
import uploadImage from '../../profileHelpers/profileBio/bioParts/uploadImage/uploadImage.vue'

export default {
    name: "tutorInfoEdit",
    components: {
        uploadImage 
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
                minimum: (value) => validationRules.minVal(value, 35),
                maximum: (value) => validationRules.maxVal(value, 1000),
                maximumChars: (value) => validationRules.maximumChars(value, 1000),
                minimumChars: (value) => validationRules.minimumChars(value, 2),
                descriptionMaxChars: (value) => validationRules.maximumChars(value, 28),
                shortParagraphMaxChars: (value) => validationRules.maximumChars(value, 96)
            }
        };
    },
    computed: {
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
            this.$store.commit('setEditDialog', false);
        }
    }
};
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';

    .tutor-edit-wrap {
        @media (max-width: @screen-xs) {
            overflow-x: hidden;
        }
        .disabled-background {
            .v-input__slot {
                background-color: #f5f5f5 !important;
            }
        }
        .leftSide {
            position: relative;
            @media (max-width: @screen-xs) {
                padding: 8px 6px;
                background: #fff;
                border-radius: 8px;
            }
            .pUb_dS_img{
                pointer-events: none !important;
            }
            .pUb_edit_img{
                position: absolute;
                right: 4px;
                text-align: center;
                width: 36px;
                height: 46px;
                border-radius: 4px;
                background-color: #fff;
                z-index: 1;
            }
        }
        .shallow-blue {
            border: 1px solid @global-blue;
            color: @color-blue-new;
            @media (max-width: @screen-xs) {
                padding: 0 16px ;
            }
        }
        .blue-btn {
            color: @color-white;
            @media (max-width: @screen-xs) {
                padding: 0 16px ;
            }
        }
        .header {
            background-color: #f0f0f7;
            width: 100%;
            color: @global-purple;        
            font-size: 18px;
            font-weight: bold;
            letter-spacing: -0.5px;
        }
        .subtitle {
            font-size: 16px;
            font-weight: bold;
            letter-spacing: -0.3px;
            color: @global-purple;
        }
        .edit-icon {
            color: @global-purple;
            font-size: 18px;
        }
    }


</style>