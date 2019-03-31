<template>
    <v-layout class="profile-bio" align-center>
        <v-flex xs12>
            <v-card class="py-4" :class="[$vuetify.breakpoint.smAndUp ?  'pl-4 px-3' : 'px-1 transparent elevation-0']">
                <v-layout v-bind="xsColumn" :class="[$vuetify.breakpoint.smAndUp ? 'align-start' : 'align-center' ]">
                    <v-flex  order-xs2 order-sm1 order-md1>
                        <user-image :isMyProfile="isMyProfile"></user-image>
                    </v-flex>
                    <v-flex xs12 order-xs1 order-sm2 order-md2 :class="[$vuetify.breakpoint.smAndUp ?  'pl-4' : 'mb-4']">
                        <v-layout class="name-price-wrap" justify-space-between align-center>
                            <v-flex xs12 sm8 md8>
                                <div class="user-name mb-2">
                                    <div class="align-start">
                                        <v-icon v-if="$vuetify.breakpoint.xsOnly && isTutorProfile" class="face-icon mr-2">sbf-face-icon</v-icon>
                                    <span class="line-height-1">{{userName}}</span>
                                        <v-icon @click="openEditInfo()"
                                                v-if="$vuetify.breakpoint.xsOnly && isMyProfile" class="edit-profile-action  ml-2">sbf-edit-icon</v-icon>
                                    </div>
                                    <div class="d-flex align-start" v-if="$vuetify.breakpoint.smAndUp">
                                        <userRank class="ml-3" :score="userScore"></userRank>
                                    </div>
                                </div>
                                <div class="text-xs-center text-sm-left text-md-left user-university text-capitalize">{{university}}</div>
                            </v-flex>
                            <div class="tutor-price mr-3">
                                <span class="tutor-price"  v-if="$vuetify.breakpoint.smAndUp && isTutorProfile">{{tutorPrice}}
                                <span class="tutor-price small-text">
                                    <span v-language:inner>app_currency_dynamic</span>
                                    <span>/</span>
                                     <span v-language:inner>profile_points_hour</span>
                                </span>
                                </span>
                                 <span class=" ml-4" v-if="$vuetify.breakpoint.smAndUp && isMyProfile">
                                     <v-icon @click="openEditInfo()" class="edit-profile-action subheading">sbf-edit-icon</v-icon>
                                 </span>
                            </div>

                        </v-layout>
                        <div class="mt-5"  v-if="$vuetify.breakpoint.smAndUp">
                            <userAboutMessage></userAboutMessage>
                        </div>
                    </v-flex>
                </v-layout>
                <v-flex>
                    <div class="tutor-price text-xs-center" v-if="$vuetify.breakpoint.xsOnly && isTutorProfile">
                                <span class="tutor-price">{{tutorPrice}}
                                <span class="tutor-price small-text">
                                      <span v-language:inner>app_currency_dynamic</span>
                                    <span>/</span>
                                     <span v-language:inner>profile_points_hour</span>
                                </span>
                                </span>
                   <span class="divider mt-4"
                                style="height: 2px; width: 44px; background-color: #979797; margin: 0 auto; display: block">
                                </span>
                    </div>
                </v-flex>
                <v-flex>
                    <div class="mt-4" v-if="$vuetify.breakpoint.xsOnly">
                        <userAboutMessage></userAboutMessage>
                    </div>
                </v-flex>

            </v-card>
        </v-flex>
        <sb-dialog
                :onclosefn="closeEditDialog"
                :activateOverlay="false"
                :showDialog="showEditDataDialog"
                :maxWidth="'760px'"
                :popUpType="'editUserInfo'"
                :content-class="'edit-dialog'"
        >
            <tutorInfoEdit v-if="isTutorProfile" :closeCallback="closeEditDialog"></tutorInfoEdit>
            <userInfoEdit v-else :closeCallback="closeEditDialog"></userInfoEdit>

        </sb-dialog>
    </v-layout>
</template>

<script>
    import { mapGetters } from 'vuex';
    import userImage from './bioParts/userImage/userImage.vue';
    import userAboutMessage from './bioParts/userAboutMessage.vue';
    import userRank from '../../../helpers/UserRank/UserRank.vue'
    import userInfoEdit from '../../profileHelpers/userInfoEdit/userInfoEdit.vue';
    import tutorInfoEdit from '../../profileHelpers/userInfoEdit/tutorInfoEdit.vue';
    import sbDialog from '../../../wrappers/sb-dialog/sb-dialog.vue';
    export default {
        name: "profileBio",
        components: {
            userImage,
            userAboutMessage,
            userRank,
            userInfoEdit,
            tutorInfoEdit,
            sbDialog
        },
        data() {
            return {
                showEditDataDialog: false,
            }
        },
        props: {
         isMyProfile: {
                type: Boolean,
                default: false
            },
        },
        computed: {
            ...mapGetters(['getProfile', 'isTutorProfile']),
            xsColumn() {
                const xsColumn = {};
                if (this.$vuetify.breakpoint.xsOnly) {
                    xsColumn.column = true;

                }
                return xsColumn
            },
            tutorPrice(){
                if (this.getProfile && this.getProfile.user && this.getProfile.user.tutorData) {
                    return this.getProfile.user.tutorData.price;
                }
                return 0
            },
            university() {
                if (this.getProfile && this.getProfile.user) {
                    return this.getProfile.user.universityName;
                }
            },
            userName(){
                if (this.getProfile && this.getProfile.user) {
                    return this.getProfile.user.firstName;
                }
            },
            userScore(){
                if (this.getProfile && this.getProfile.user) {
                    return this.getProfile.user.score;
                }
            }
        },
        methods: {
            openEditInfo() {
                    this.showEditDataDialog = true;
            },
            closeEditDialog() {
                this.showEditDataDialog = false;
            },
        },
    }
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';

    .profile-bio {
        max-width: 760px;
        .user-name {
            display: flex;
            flex-direction: row;
            align-items: flex-end;
            font-family: @fontOpenSans;
            font-size: 18px;
            font-weight: bold;
            letter-spacing: -0.4px;
            color: @profileTextColor;
            @media(max-width: @screen-xs){
                justify-content: center;
                align-items: center;
            }
            .face-icon{
                font-size: 18px;
            }

        }
        .tutor-price{
            font-family: @fontOpenSans;
            font-weight: bold;
            font-size: 20px;
            color: @profileTextColor;
            @media(max-width: @screen-xs){
                font-size: 26px;
            }
            .small-text{
                font-size: 13px;
            }

        }
        .edit-profile-action{
            color: @purpleLight;
            opacity: 0.41;
            font-size: 20px;
            cursor: pointer;
           @media(max-width: @screen-xs){
               color: @purpleDark;
           }
        }
        .line-height-1{
            line-height: 1;
        }
        .user-university {
            font-size: 14px;
            letter-spacing: -0.3px;
            color: @textColor;

        }
    }

</style>