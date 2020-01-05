<template>
    <v-card class="verify-creation-wrap">
        <v-layout class="close-toolbar pl-4 pr-4" style="width:100%;" align-center justify-end>
            <v-flex shrink class="mr-2">
                <v-icon class="subtitle-1 course-close-icon" @click="closeDialog()" >sbf-close</v-icon>
            </v-flex>
        </v-layout>

        <v-layout align-center justify-center :class="{'shrink': $vuetify.breakpoint.xsOnly}">
            <v-flex xs12 sm12  class="text-center">
                <thinkIcon></thinkIcon>
            </v-flex>
        </v-layout>
        <v-layout align-center justify-center :class="[ $vuetify.breakpoint.xsOnly ? 'shrink mt-4': '']">
            <v-flex class="text-center">
                <span class="font-weight-bold heading-text" v-language:inner>courses_before creating</span>
            </v-flex>
        </v-layout>
        <v-layout align-center justify-center :class="[ $vuetify.breakpoint.xsOnly ? 'shrink mt-4': '']">
            <v-flex xs12  sm5  class="text-center">
                <p class="body-2 mb-1" v-language:inner>courses_check_mistakes</p>
                <p class="body-2 mb-1" v-language:inner>courses_remove_spaces</p>
                <p class="body-2 mb-1" v-language:inner>courses_find_similar</p>
            </v-flex>
        </v-layout>
        <v-layout align-center justify-center :class="[ $vuetify.breakpoint.xsOnly ? 'shrink mt-5': '']">
            <v-flex shrink xs6  sm3>
                <button @click="goTocreation()" class="min-width shrink outline d-flex align-center justify-center py-2 px-3">
                    <span class="font-weight-bold text-capitalize" v-language:inner>courses_create_course</span>
                </button>
            </v-flex>
            <v-flex shrink xs5  sm3>
                <button @click="closeDialog()" class="min-width solid d-flex align-center justify-center py-2 px-3">
                    <span class="font-weight-bold text-capitalize" v-language:inner>courses_try_again</span>
                </button>
            </v-flex>
        </v-layout>
    </v-card>
    
</template>

<script>
    import {  mapActions } from 'vuex';
    import thinkIcon from  '../images/thinking.svg'
    export default {
        name: "verifyCreation",
        components:{ thinkIcon },
        methods: {
            ...mapActions(['updateVerification', 'changeCreateDialogState']),
           goTocreation(){
               this.updateVerification(true)
           },
           closeDialog(){
               this.changeCreateDialogState(false);
               this.$root.$emit('courseDialogClosed', true);
           }
        },
    };
</script>

<style lang="less">
    @import '../../../styles/mixin.less';
    .verify-creation-wrap{
        min-height: 340px;
        .close-toolbar{
            max-height: 54px;
            width: 100%;
            background-color: @color-white;
        }
        .outline{
            border-radius: 22px;
            font-size: 14px;
            border: solid 1px @global-purple;
            color: @global-purple;
            background-color: transparent;
        }
        .min-width{
            width: 146px;
        }
        .heading-text{
            font-size: 18px;
        }
        .solid{
            border-radius: 22px;
            font-size: 14px;
            color: @color-white;
            background-color: @global-purple;
        }
    }

</style>