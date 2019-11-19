<template>
    <div class="student-tutor-wrap">
        <v-container align-center align-content-center justify-center>
            <v-layout row wrap align-center justify-center class="pt-4">
                <v-flex xs12   class="text-center pb-2">
                    <span class="body-2 text-black" v-language:inner>studentOrTutor_title</span>
                </v-flex>
                <v-flex xs12   class="text-center">
                    <span class="headline font-weight-bold text-black" v-language:inner>studentOrTutor_subtitle</span>
                </v-flex>
            </v-layout>
            <v-layout row wrap align-center justify-center class="pt-5">
                <v-flex xs12   class="text-center pb-4">
                    <span class="body-2 font-weight-bold text-black" v-language:inner>studentOrTutor_currently</span>
                </v-flex>
                <v-flex xs12 sm2  shrink class="text-center d-inline-flex btn-wrap">
                    <v-btn @click="goToUniSelect()" class="sb-yellow-btn elevation-0"
                           :class="{'wide-btn ': $vuetify.breakpoint.smAndUp}">
                        <span class="subtitle-1 font-weight-bold text-capitalize" v-language:inner>studentOrTutor_btn_student</span>
                    </v-btn>
                </v-flex>
                <v-flex xs12 sm2  shrink class="text-center d-inline-flex btn-wrap">
                    <v-btn @click="openBecomeTutor()" class="sb-yellow-btn elevation-0"
                           :class="{'wide-btn ': $vuetify.breakpoint.smAndUp}">
                        <span class="subtitle-1 font-weight-bold text-capitalize" v-language:inner>studentOrTutor_btn_tutor</span>
                    </v-btn>
                </v-flex>
            </v-layout>
            <v-layout align-center justify-center>
                <v-flex xs12   class="text-center">
                    <img v-if="$vuetify.breakpoint.xsOnly" class="people-img mobile"
                         src="./images/people-background-mobile.png" alt="student tutor study">
                    <img v-else class="people-img" src="./images/people-background.png" alt="student tutor study">
                </v-flex>
            </v-layout>
        </v-container>

    </div>
</template>

<script>
    import { mapActions, mapGetters } from 'vuex';

    export default {
        name: "studentOrTutor",
        computed: {
            ...mapGetters(['accountUser']),
            isTutor() {
                return this.accountUser.isTutor;
            }
        },
        methods: {
            ...mapActions(['updateTutorDialog']),
            goToUniSelect() {
                this.$router.push({name: 'addUniversity'});
            },
            openBecomeTutor() {
                this.isTutor ? this.$router.push({name: 'editCourse'}) : this.updateTutorDialog(true);
            }
        },
    };
</script>

<style lang="less">
    @import '../../styles/mixin.less';

    .student-tutor-wrap {
        .people-img {
            max-width: 544px;
            height: auto;
            &.mobile {
                width: 100%;
                height: auto;
            }
        }
        .theme--light.v-btn:not(.v-btn--icon):not(.v-btn--flat) {
            &.sb-yellow-btn {
                background-color: @yellowColor;
                color: @global-purple;
            }
            &.wide-btn {
                width: 162px;
                height: 42px;
                flex-grow: 0 !important;
            }
            &:not(.wide-btn) {
                max-width: 206px;
            }
        }
        .btn-wrap {
            align-items: center;
            justify-content: flex-end;
            &:last-child {
                justify-content: flex-start;
            }
            @media (max-width: @screen-xs) {
                justify-content: center;
                &:last-child {
                    justify-content: center;
                }
            }
        }
        .text-black {
            color: @textColor;
        }

    }

</style>