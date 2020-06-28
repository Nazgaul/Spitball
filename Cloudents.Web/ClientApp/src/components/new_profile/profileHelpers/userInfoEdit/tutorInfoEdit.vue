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
                        v-model="description"
                        :rules="[rules.descriptionMaxChars]"
                        counter="25"
                        class="tutor-edit-description"
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
                        counter="80"
                        class="tutor-edit-description"
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
    import { mapGetters } from 'vuex';
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
                editedDescription: '',
                editShortParagraph: '',
                editedFirstName: '',
                editedLastName: '',
                rules: {
                    required: (value) => validationRules.required(value),
                    minimum: (value) => validationRules.minVal(value, 35),
                    maximum: (value) => validationRules.maxVal(value, 1000),
                    maximumChars: (value) => validationRules.maximumChars(value, 1000),
                    minimumChars: (value) => validationRules.minimumChars(value, 2),
                    // descriptionMinChars: (value) => validationRules.minimumChars(value, 2),
                    descriptionMaxChars: (value) => validationRules.maximumChars(value, 25),
                    shortParagraphMaxChars: (value) => validationRules.maximumChars(value, 80),
                    integer: (value) => validationRules.integer(value)
                },
                valid: false,
                btnLoading: false
            };
        },
        computed: {
            ...mapGetters(['getProfile','accountUser', 'isFrymo']),
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
                    return this.getProfile.user.firstName;
                },
                set(newVal) {
                    this.editedFirstName = newVal;
                }
            },
            lastName: {
                get() {
                    return this.getProfile.user.lastName;
                },
                set(newVal) {
                    this.editedLastName = newVal;
                }
            },
            description: {
                get() {
                    return this.$store.getters.getProfileDescription;
                },
                set(newVal) {
                    this.editedDescription = newVal;
                }
            }
        },
        methods: {
            saveChanges() {
                if(this.$refs.formTutor.validate()) {
                    debugger;
                    let firstName = this.editedFirstName || this.firstName;
                    let lastName = this.editedLastName || this.lastName;
                    let shortParagraph = this.editShortParagraph || this.shortParagraph; //2
                    let bio = this.editedBio || this.bio; //3
                    let description = this.editedDescription || this.description; //TITLE
                    let editsData = {
                        name: `${firstName} ${lastName}`,
                        lastName,
                        firstName,
                        shortParagraph,
                        bio,
                        description,
                    };
                    this.btnLoading = true;
                    let serverFormat = {
                        firstName,
                        lastName,
                        shortParagraph,
                        bio,
                        description,
                    };
                    this.$store.dispatch('saveUserInfo', serverFormat)
                        .then(() => {
                            //update profile store
                            this.$store.commit('updateEditedData',editsData);
                            this.closeDialog();
                        }, (error) => {
                            console.log('Error', error);
                            //TODO : error callback
                        }).finally(() => {
                            this.btnLoading = false;
                        });
                }
            },
            closeDialog() {
                this.$store.commit('setEditDialog', false);
            }
        },
        created() {
            this.editedBio = this.getProfile.user.tutorData.bio || '';
            this.editedDescription = this.getProfile.user.description || '';
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