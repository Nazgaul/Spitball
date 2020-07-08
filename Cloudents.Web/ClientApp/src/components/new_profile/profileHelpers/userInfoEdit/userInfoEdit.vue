<template>
    <v-dialog :value="true" :content-class="'edit-dialog'" persistent max-width="760px" :fullscreen="$vuetify.breakpoint.xsOnly">
        <v-card class="user-edit-wrap pb-4">
            <v-form class="userEditForm d-flex d-sm-block flex-column justify-space-between pb-4 pb-sm-0" v-model="validUserForm" ref="formUser" @submit.prevent>
                <div class="userEditFlexWrap">
                    <v-layout class="header pa-4 pt-3 mb-4">
                        <v-flex>
                            <v-icon class="edit-icon mr-2">sbf-edit-icon</v-icon>
                            <span>{{$t('profile_edit_user_profile_title')}}</span>
                        </v-flex>
                    </v-layout>
                    <v-layout class="px-3 mt-3">
                        <div class="leftSide me-3 d-inline-block">
                            <uploadImage
                                v-show="avatarLoading || !$store.getters.getAccountImage"
                                @setProfileAvatarLoading="val => avatarLoading = val"
                                class="editImage"
                                sel="photo"
                            />
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
                                :loading="avatarLoading"
                                @setAvatarLoaded="val => avatarLoading = val"
                            />
                        </div>
                        <v-layout wrap>
                            <v-flex xs12 :class="{'pr-2': $vuetify.breakpoint.smAndUp}">
                                <!-- <v-layout column> -->
                                    <v-flex xs12 class="pl-2 mb-2">
                                        <span class="subtitle">{{$t('profile_personal_details')}}</span>
                                    </v-flex>
                                    <v-flex xs12>
                                        <v-text-field
                                                :rules="[rules.required, rules.minimumChars]"
                                                :label="$t('profile_firstName_label')"
                                                class="tutor-edit-firstname"
                                                v-model="firstName"
                                                outlined
                                        ></v-text-field>
                                    </v-flex>
                                <!-- </v-layout> -->
                            </v-flex>
                            <v-flex xs12 :class="[ $vuetify.breakpoint.xsOnly ? 'mt-2 mr-0' : 'pr-2']">
                                <!-- <v-layout column> -->
                                    <!-- <v-flex v-if="$vuetify.breakpoint.smAndUp" xs12 class="mb-2 pl-2" grow>
                                        <span class="subtitle" style="visibility: hidden">hidden</span>
                                    </v-flex> -->
                                    <v-flex>
                                        <v-text-field
                                                :rules="[rules.required, rules.minimumChars]"
                                                :label="$t('profile_lastName_label')"
                                                class="tutor-edit-lastname"
                                                v-model="lastName"
                                                outlined
                                        ></v-text-field>
                                    </v-flex>
                                <!-- </v-layout> -->
                            </v-flex>
                        </v-layout>
                    </v-layout>
                </div>

                <div class="text-center mt-5">
                    <v-btn :disabled="btnLoading" width="120" depressed color="#4452fc" class="shallow-blue ml-0" rounded outlined primary @click="$store.commit('setComponent', '')">
                        <span v-t="'cancel'"></span>
                    </v-btn>
                    <v-btn class="blue-btn white--text ms-4" width="120" depressed color="#4452fc" rounded @click="saveChanges" :loading="btnLoading">
                        <span v-t="'save'"></span>
                    </v-btn>
                </div>
                <!-- OLD BUTTONS ASK SHIRAN AND RAM -->
                <!-- <v-layout  align-center class="bottomActions px-3" :class="[$vuetify.breakpoint.xsOnly ? 'justify-space-between ' : 'justify-end']">
                    <v-flex xs5 sm2  >
                        <v-btn class="shallow-blue ml-0" rounded outlined primary @click="$store.commit('setComponent', '')">
                            <span>{{$t('profile_btn_cancel')}}</span>
                        </v-btn>
                    </v-flex>
                    <v-flex xs5 sm2  :class="{'mr-4': $vuetify.breakpoint.smAndUp}">
                        <v-btn class="blue-btn  ml-0" rounded @click="saveChanges()" :loading="btnLoading">
                            <span>{{$t('profile_btn_save_changes')}}</span>
                        </v-btn>
                    </v-flex>
                </v-layout> -->
            </v-form>
        </v-card>
    </v-dialog>
</template>

<script>
import { validationRules } from "../../../../services/utilities/formValidationRules";
import uploadImage from '../../profileHelpers/profileBio/bioParts/uploadImage/uploadImage.vue';

export default {
    name: "userInfoEdit",
    components: {
        uploadImage
    },
    data() {
        return {
            editedLastName:'',
            editedFirstName:'',
            rules: {
                required:(value)=> validationRules.required(value),
                minimumChars: (value) => validationRules.minimumChars(value, 2),
            },
            validUserForm: false,
            btnLoading: false,
            avatarLoading: false,
        }
    },
    computed: {
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly
        },
        firstName:{
            get(){
                return this.$store.getters.getAccountFirstName
            },
            set(newVal){
                this.editedFirstName = newVal;
            }
        },
        lastName:{
            get(){
                return this.$store.getters.getAccountLastName
            },
            set(newVal){
                this.editedLastName = newVal;
            }
        },
    },
    methods: {
        saveChanges() {
            if(this.$refs.formUser.validate()) {
                this.btnLoading = true;
                let studentInfo = {
                    firstName: this.editedFirstName || this.firstName,
                    lastName: this.editedLastName || this.lastName
                };
                //TODO: Account new store clean @idan
                this.$store.dispatch('saveUserInfo', studentInfo).then(() => {
                    this.btnLoading = false;
                    this.$store.commit('setComponent', '')
                })
            }
        }
    }
}
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';

    .user-edit-wrap {
        height: 100%;
        @media(max-width: @screen-xs){
            overflow-x: hidden;
        }
        // .disabled-background{
        //     .v-input__slot{
        //         background-color: #f5f5f5!important;
        //     }
        // }
        // .shallow-blue{
        //     border: 1px solid  @color-blue-new;
        //     color:  @color-blue-new;
        //     @media(max-width: @screen-xs){
        //         min-width: 100%;
        //         padding: 0 16px ;
        //         border-radius: 0;
        //     }
        // }
        //vuetify overwite
        // .blue-btn{
        //     background-color:  @color-blue-new!important;
        //     color: @color-white;
        //     box-shadow: none!important;
        //     @media(max-width: @screen-xs){
        //         min-width: 100%;
        //         padding: 0 16px ;
        //         border-radius: 0;
        //     }
        // }
        .userEditForm {
            height: inherit;
            .userEditFlexWrap {
                .header {
                    background-color: #f0f0f7;
                    width: 100%;
                    max-height: 50px;
                    color: @global-purple;   
                    font-size: 18px;
                    font-weight: bold;
                    letter-spacing: -0.5px;
                }
                .subtitle{
                    font-size: 16px;
                    font-weight: bold;
                    letter-spacing: -0.3px;
                    color: @global-purple;
                }
                .edit-icon {
                    color: @global-purple;
                    font-size: 18px;
                }
                .leftSide {
                    position: relative;
                    width: max-content;
                    margin: 0 auto;
                    @media (max-width: @screen-xs) {
                        padding: 8px 6px;
                        background: #fff;
                        border-radius: 8px;
                    }
                    // .pictureTitle {
                    //     .responsive-property(font-size, 18px, null, 16px);
                    //     font-weight: 600;
                    //     color: #131415;
                    // }
                    .pUb_dS_img {
                        pointer-events: none !important;
                    }
                    .editImage {
                        position: absolute;
                        text-align: center;
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
            }
        }
        .v-text-field--outline > .v-input__control > .v-input__slot {
            border: 1px solid rgba(0, 0, 0, 0.19);
            &:hover {
                border: 1px solid rgba(0, 0, 0, 0.19) !important;
            }
        }
        // .bottomActions {
        //     margin-top: 60px;
        // }

        // .shallow-blue {
        //     border: 1px solid @global-blue;
        //     color: @color-blue-new;
        //     @media (max-width: @screen-xs) {
        //         padding: 0 16px ;
        //     }
        // }
    }
</style>