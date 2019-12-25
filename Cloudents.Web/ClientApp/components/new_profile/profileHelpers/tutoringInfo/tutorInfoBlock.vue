<template>
    <v-layout class="tutoring-info-section" align-center>
        <v-flex xs12>
            <v-card class="tutoring-info-card"
                    :class="[$vuetify.breakpoint.xsOnly ? 'elevation-0 px-0 py-2': 'py-4']">
                <v-flex class="hidden-sm-and-down">
                    <div>
                        <div class="mb-2 text-xs-center px-3">
                        <span class="tutoring-info-heading font-italic" v-language:inner>profile_tutor_sidebar_title
                        </span>
                        </div>
                    </div>
                </v-flex>
                <v-flex xs12 :class="[$vuetify.breakpoint.xsOnly ? 'mobile-btn-fixed-bottom py-0 mb-0' : 'py-4 mb-3']">
                    <contactBtn v-if="!isMyProfile" ></contactBtn>
                </v-flex>
              
            </v-card>
        </v-flex>
    </v-layout>
</template>

<script>
    import contactBtn from '../../profileHelpers/contactBtn/contactBtn.vue';
    import {mapGetters } from 'vuex';
    export default {
        name: "tutorInfoBlock",
        components: {contactBtn},
        computed: {
            ...mapGetters(['getProfile', 'accountUser']),
            isMyProfile() {
                if(!!this.getProfile) {
                    return this.accountUser && this.accountUser.id && this.getProfile ? this.getProfile.user.id == this.accountUser.id : false;
                }
                return false
            }
        },
    }
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';

    .tutoring-info-section {
        flex-direction: column;
        min-width: 260px;
        @media (max-width: @screen-xs) {
            background-color: transparent;
            flex-direction: row;
        }
        .mobile-btn-fixed-bottom {
            align-items: baseline;
            position: fixed;
            bottom: 62px;
            right: 0;
            width: 100%;
            z-index: 9;
            margin: 0;
            padding: 0;
            background-color: @systemBackgroundColor;
            .ct-btn{
                width: 98%;
                border-radius: 4px;
                box-shadow: 0 3px 8px 0 rgba(0, 0, 0, 0.22);
                margin: 0 auto;

            }
        }
        .tutoring-info-card {
            display: flex;
            align-items: center;
            justify-content: center;
            flex-direction: column;
            padding-left: 8px;
            padding-right: 8px;
            box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.18);
            @media (max-width: @screen-xs) {
                background: transparent;
                flex-direction: row;
            }
        }
        .bottom-section {
            display: flex;
            flex-direction: column;
            width: 100%;
            justify-content: space-between;
            @media (max-width: @screen-xs) {
                flex-direction: row;
                padding: 0 !important;
            }
        }
        .info-item {
            display: flex;
            justify-content: space-between;
            @media (max-width: @screen-xs) {
                flex-direction: column;
                box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.16);
                background-color: @color-white;
                border-radius: 4px;
                min-height: 76px;
                min-width: 116px;
                padding: 12px;
                margin-bottom: 0;
                margin-left: 8px;
                &:first-child {
                    margin-left: 0;
                }

            }
        }
        .tutoring-info-heading {
            font-size: 14px;
            font-weight: 500;
            font-style: italic;
            color: @global-purple;
        }
        .tutoring-info-label {
            font-size: 12px;
            line-height: 1.46;
            color: @textColor;
            @media (max-width: @screen-xs) {
                padding-top: 10px;
                padding-bottom: 20px;
                font-size: 12px;
                line-height: 1.58;
                color: rgba(0, 0, 0, 0.54);
            }
        }
        .tutoring-info-value {
            font-size: 13px;
            font-weight: 600;
            line-height: 1.46;
            color: @global-purple;
            @media (max-width: @screen-xs) {
                padding-bottom: 20px;

            }
        }
    }

</style>