<template>
    <div class="request-box-wrap" :class="[$vuetify.breakpoint.xsOnly ? 'px-3' : 'px-4']">
        <v-layout align-center justify-start class="pt-3">
            <!--<v-flex xs1 md1 sm1 shrink v-if="$vuetify.breakpoint.smAndUp && accountUser" >-->
            <v-flex xs1 md1 sm1 shrink >
                <userAvatar
                        class="avatar-circle  mr-2"
                        :userImageUrl="userImageUrl"
                        :user-name="userName"
                        :user-id="userID"></userAvatar>
            </v-flex>
            <v-flex xs10 sm11 md11 grow class="text-xs-left" :class="[$vuetify.breakpoint.xsOnly ? 'ml-2 pl-1' : '']">
                <span class="subheading font-weight-bold request-box-title" v-language:inner>requestActions_title
                </span>
            </v-flex>
        </v-layout>
        <v-layout align-space-between class="pt-3 pb-3">
            <v-flex sm4 md4 class="btn-wrap text-xs-left">
                <v-btn round class="light-btn elevation-0 ma-0" @click="openAskQuestion()">
                    <v-icon class="light-btn-icon subheading mr-2">sbf-message-icon-new</v-icon>
                    <span v-show="$vuetify.breakpoint.smAndUp" class="text-capitalize body-2 font-weight-bold"
                          v-language:inner>requestActions_btn_ask</span>
                    <span v-show="$vuetify.breakpoint.xsOnly" class="text-capitalize body-2 font-weight-bold"
                          v-language:inner>requestActions_btn_ask_mob</span>
                </v-btn>
            </v-flex>
            <v-flex sm4 md4 class="btn-wrap text-xs-center">
                <v-btn round class="light-btn elevation-0 ma-0" @click="openUpload()">
                    <v-icon class="light-btn-icon subheading mr-2">sbf-upload-icon</v-icon>
                    <span v-show="$vuetify.breakpoint.smAndUp" class="text-capitalize body-2 font-weight-bold"
                          v-language:inner>requestActions_btn_upload</span>
                    <span v-show="$vuetify.breakpoint.xsOnly" class="text-capitalize body-2 font-weight-bold"
                          v-language:inner>requestActions_btn_upload_mob</span>
                </v-btn>
            </v-flex>
            <v-flex sm4 md4 class="btn-wrap text-xs-right">
                <v-btn round class="light-btn elevation-0 ma-0" @click="openRequestTutor()">
                    <v-icon class="light-btn-icon subheading mr-2">sbf-person-icon</v-icon>
                    <span v-show="$vuetify.breakpoint.smAndUp" class="text-capitalize body-2 font-weight-bold"
                          v-language:inner>requestActions_btn_tutor</span>
                    <span v-show="$vuetify.breakpoint.xsOnly" class="text-capitalize body-2 font-weight-bold"
                          v-language:inner>requestActions_btn_tutor_mob</span>
                </v-btn>
            </v-flex>
        </v-layout>
    </div>
</template>

<script>
    import { mapActions, mapGetters } from 'vuex';
    import userAvatar from '../helpers/UserAvatar/UserAvatar.vue';

    export default {
        name: "requestActions",
        components: {userAvatar},
        computed: {
            ...mapGetters(['accountUser', 'getSchoolName', 'getSelectedClasses']),
            userImageUrl() {
                if(this.accountUser && this.accountUser.image.length > 1) {
                    return `${this.accountUser.image}`;
                }
                return '';
            },
            userName(){
                if(this.accountUser && this.accountUser.name.length > 1) {
                    return `${this.accountUser.name}`;
                }
            },
            userID(){
                if(this.accountUser && this.accountUser.id) {
                    return this.accountUser.id
                }
            }
        },
        methods: {
            ...mapActions([
                              'updateNewQuestionDialogState',
                              'updateLoginDialogState',
                              'updateUserProfileData',
                              'setReturnToUpload',
                              'updateDialogState',
                              'updateRequestDialog'
                          ]),
            openAskQuestion() {
                if(this.accountUser == null) {
                    this.updateLoginDialogState(true);
                    //set user profile
                    this.updateUserProfileData('profileHWH');
                } else {
                    this.updateNewQuestionDialogState(true);
                }

            },
            openUpload() {
                let schoolName = this.getSchoolName;
                if(this.accountUser == null) {
                    this.updateLoginDialogState(true);
                } else if(!schoolName.length) {
                    this.$router.push({name: 'addUniversity'});
                    this.setReturnToUpload(true);
                } else if(!this.getSelectedClasses.length) {
                    this.$router.push({name: 'addCourse'});
                    this.setReturnToUpload(true);
                } else if(schoolName.length > 0 && this.getSelectedClasses.length > 0) {
                    this.updateDialogState(true);
                    this.setReturnToUpload(false);
                }
            },
            openRequestTutor() {
               
                if(this.accountUser == null) {
                    this.updateLoginDialogState(true);
                } else {
                    if(this.getSelectedClasses.length){
                         debugger;
                        this.updateRequestDialog(true);
                    }else {
                        this.$router.push({name: 'addCourse'});
                    }
                }
                console.log('open tutor request dialog');
            }
        },
    };
</script>

<style lang="less">
    @import '../../styles/mixin.less';

    .request-box-wrap {
        box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.24);
        background-color: @color-white;
        border-radius: 4px;
        .request-box-title {
            color: @textColor;
            letter-spacing: -0.4px;
        }
        .light-btn {
            border-radius: 16px;
            height: 32px;
            color: @color-white;
            width: 189px;
            background: @btnGreen !important;
            @media (max-width: @screen-xs) {
                width: 110px;
            }
            .light-btn-icon {
                color: @color-white;
            }
        }

    }

</style>