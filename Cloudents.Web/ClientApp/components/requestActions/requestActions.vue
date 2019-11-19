<template>
    <div class="request-box-wrap" :class="[$vuetify.breakpoint.xsOnly ? 'px-2 mx-2' : 'px-3']">
        <v-layout align-center justify-start class="pt-4">
            <v-flex class="avatar-holder px-1" v-if="accountUser" >
                <userAvatar
                        class="avatar-circle  mr-2"
                        :userImageUrl="userImageUrl"
                        :user-name="userName"
                        :user-id="userID"></userAvatar>
            </v-flex>
            <v-flex xs10 sm11  grow class="text-xs-left" :class="[$vuetify.breakpoint.xsOnly ? 'ml-2 pl-1' : '']">
                <span class="subtitle-1 font-weight-bold request-box-title" v-language:inner>requestActions_title
                </span>
            </v-flex>
        </v-layout>
        <v-layout align-space-between class="pt-4 pb-4 buttons-layout text-center" justify-space-between>
            <v-flex sm4 class="btn-wrap" shrink>
                <v-btn rounded class="light-btn elevation-0 ma-0" @click="openAskQuestion()">
                    <v-icon class="light-btn-icon  mr-2">sbf-message-icon-new</v-icon>
                    <span v-show="$vuetify.breakpoint.smAndUp" class="body-2 font-weight-medium"
                          v-language:inner>requestActions_btn_ask</span>
                    <span v-show="$vuetify.breakpoint.xsOnly" class="body-2 font-weight-medium"
                          v-language:inner>requestActions_btn_ask_mob</span>
                </v-btn>
            </v-flex>
            <v-flex sm4 class="btn-wrap" shrink>
                <v-btn rounded class="light-btn elevation-0 ma-0" @click="openUpload()">
                    <v-icon class="light-btn-icon  mr-2">sbf-upload-icon</v-icon>
                    <span v-show="$vuetify.breakpoint.smAndUp" class="body-2 font-weight-medium"
                          v-language:inner>requestActions_btn_upload</span>
                    <span v-show="$vuetify.breakpoint.xsOnly" class="body-2 font-weight-medium"
                          v-language:inner>requestActions_btn_upload_mob</span>
                </v-btn>
            </v-flex>
            <v-flex sm4 class="btn-wrap" shrink>
                <v-btn rounded class="light-btn elevation-0 ma-0" @click="openRequestTutor()">
                    <v-icon class="light-btn-icon  mr-2">sbf-person-icon</v-icon>
                    <span v-show="$vuetify.breakpoint.smAndUp" class="body-2 font-weight-medium"
                          v-language:inner>requestActions_btn_tutor</span>
                    <span v-show="$vuetify.breakpoint.xsOnly" class="body-2 font-weight-medium"
                          v-language:inner>requestActions_btn_tutor_mob</span>
                </v-btn>
            </v-flex>
        </v-layout>
    </div>
</template>

<script>
    import { mapActions, mapGetters } from 'vuex';
    import userAvatar from '../helpers/UserAvatar/UserAvatar.vue';
    import analyticsService from '../../services/analytics.service';

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
                              'setReturnToUpload',
                              'updateDialogState',
                              'updateRequestDialog',
                              'setTutorRequestAnalyticsOpenedFrom'
                          ]),
            openAskQuestion() {
                // analyticsService.sb_unitedEvent('Action Box', 'Request_T', `USER_ID:${analyticsObject.userId}, T_Course:${analyticsObject.course}`);
                if(this.accountUser == null) {
                    this.updateLoginDialogState(true);
                } else {
                    this.updateNewQuestionDialogState(true);
                }

            },
            openUpload() {
                // analyticsService.sb_unitedEvent('Action Box', 'Request_T', `USER_ID:${analyticsObject.userId}, T_Course:${analyticsObject.course}`);
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
                analyticsService.sb_unitedEvent('Tutor_Engagement', 'request_box');
                if(this.accountUser == null) {
                    //this.updateLoginDialogState(true);
                    this.setTutorRequestAnalyticsOpenedFrom({
                        component: 'actionBox',
                        path: this.$route.path
                    });
                    this.updateRequestDialog(true);
                } else {
                    if(this.getSelectedClasses.length){
                        this.setTutorRequestAnalyticsOpenedFrom({
                            component: 'actionBox',
                            path: this.$route.path
                        });
                        this.updateRequestDialog(true);
                    }else {
                        this.$router.push({name: 'addCourse'});
                    }
                }
            }
        },
    };
</script>

<style lang="less">
    @import '../../styles/mixin.less';

    .request-box-wrap {
        // box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.24);
        background-color: @color-white;
        border-radius: 4px;
        .buttons-layout{
            @media (max-width: 344px) {
               display:flex;
               flex-direction: column;
               div{
                   margin-bottom: 4px;
               }
            }
        }
        .request-box-title {
            color: @textColor;
            letter-spacing: -0.4px;
        }
        .avatar-holder{
            max-width: 32px;
            margin-right: 14px;
        }
        .light-btn {
            text-transform: none;
            border-radius: 16px;
            height: 32px;
            color: @color-white;
            width: 194px;
            background: #514f7d  !important;
            .v-btn__content{
                align-items: flex-end;
            }
            @media (max-width: @screen-xs) {
                width: 100%; //keep it for mobile
            }
            .light-btn-icon {
                color: @color-white;
                font-size: 14px;
                line-height: 17px;
            }
        }

    }

</style>