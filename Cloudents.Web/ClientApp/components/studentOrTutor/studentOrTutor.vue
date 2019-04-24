<template>
    <div class="student-tutor-wrap">
        <v-container align-center align-content-center justify-center>
            <v-layout row wrap align-center justify-center class="pt-3">
                <v-flex xs12 sm12 md12 class="text-xs-center pb-2">
                    <span class="body-2 text-black" v-language:inner>studentOrTutor_title</span>
                </v-flex>
                <v-flex xs12 sm12 md12 class="text-xs-center">
                    <span class="headline font-weight-bold text-black" v-language:inner>studentOrTutor_subtitle</span>
                </v-flex>
            </v-layout>
            <v-layout row wrap  align-center justify-center class="pt-5">
                <v-flex xs12 sm12 md12 class="text-xs-center pb-4">
                    <span class="body-2 font-weight-bold text-black" v-language:inner>studentOrTutor_currently</span>
                </v-flex>
                <v-flex xs2 sm2 md2  shrink class="text-xs-center d-inline-flex align-center justify-end">
                    <v-btn class="sb-yellow-btn elevation-0" :class="{'wide-btn ': $vuetify.breakpoint.smAndUp}">
                        <span class="subheading font-weight-bold text-capitalize" v-language:inner>studentOrTutor_btn_student</span>
                    </v-btn>
                </v-flex>
                <v-flex xs2 sm2 md2 shrink class="text-xs-center d-inline-flex align-center justify-start">
                    <v-btn class="sb-yellow-btn elevation-0" :class="{'wide-btn ': $vuetify.breakpoint.smAndUp}">
                        <span class="subheading font-weight-bold text-capitalize" v-language:inner>studentOrTutor_btn_tutor</span>
                    </v-btn>
                </v-flex>
            </v-layout>
            <v-layout align-center justify-center>
            <v-flex xs12 sm12 md12 class="text-xs-center">
                <img class="people-img" src="./images/people-background.png" alt="student tutor study">
            </v-flex>
            </v-layout>
        </v-container>

    </div>
</template>

<script>
    export default {
        name: "studentOrTutor"
    };
</script>

<style lang="less">
    @import '../../styles/mixin.less';
    .student-tutor-wrap{
        .people-img{
            max-width: 544px;
            height: auto;
        }
        .theme--light.v-btn:not(.v-btn--icon):not(.v-btn--flat){
            &.sb-yellow-btn{
                background-color: @yellowColor;
                color: @profileTextColor;
            }
            &.wide-btn{
                width: 162px;
                height: 42px;
                flex-grow: 0!important;
            }
        }

        .text-black{
            color: @textColor;
        }

    }

</style>