<template>
    <div class="become-final-wrap">
        <v-layout row wrap>
            <v-flex xs12 class="text-xs-center mb-4">
               <v-icon class="face-icon">sbf-face-icon</v-icon>
            </v-flex>

            <v-flex xs12 class="text-xs-center text-capitalize mb-2 mt-1" >
                <span class="final-text font-weight-bold" v-language:inner>becomeTutor_oh</span>
            </v-flex>

            <v-flex xs12 class="text-xs-center mb-4">
                <span class="final-text font-weight-bold" v-language:inner>becomeTutor_tutor_now</span>
            </v-flex>

            <v-flex xs12 class="text-xs-center">
                <v-btn @click="goToCourses()" round class="add-btn text-uppercase">
                    <span class="font-weight-bold" v-language:inner>becomeTutor_btn_add_courses_teach</span>
                </v-btn>
            </v-flex>
        </v-layout>
    </div>
</template>

<script>
    import {mapActions} from 'vuex';
    export default {
        name: "finalStep",
        methods: {
            ...mapActions(['updateTutorDialog']),
            goToCourses() {
                this.$router.push({name: 'editCourse'});
                this.updateTutorDialog(false)

            }
        },
    };
</script>

<style lang="less">
    @import '../../../styles/mixin.less';
    .become-final-wrap{
        padding: 106px 0;
            .face-icon{
                font-size: 44px;
                color: @profileTextColor;
            }
        .final-text{
            font-size: 20px;
            color: @profileTextColor;
        }
        .theme--light.v-btn:not(.v-btn--icon):not(.v-btn--flat){
            &.add-btn{
                height: 41px;
                border-radius: 20.5px;
                background-color: @profileTextColor!important;
                color: @color-white;
                width: 266px;
            }
            @media(max-width: @screen-xs){
                width: unset;
            }
        }
    }

</style>