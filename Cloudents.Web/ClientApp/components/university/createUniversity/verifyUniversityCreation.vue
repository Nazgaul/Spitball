<template>
    <v-card class="verify-creation-wrap">
        <v-layout class="close-toolbar pl-4 pr-4" style="width:100%;" align-center justify-end>
            <v-flex shrink class="mr-2">
                <v-icon class="subtitle-1 course-close-icon" @click="closeDialog()" >sbf-close</v-icon>
            </v-flex>
        </v-layout>

        <v-layout align-center justify-center :class="{'shrink': $vuetify.breakpoint.xsOnly}">
            <v-flex xs12 class="text-center">
                <thinkIcon></thinkIcon>
            </v-flex>
        </v-layout>
        <v-layout align-center justify-center :class="[ $vuetify.breakpoint.xsOnly ? 'shrink mt-4': '']">
            <v-flex class="text-center">
                <span class="font-weight-bold heading-text" v-language:inner>university_new_title</span>
            </v-flex>
        </v-layout>
        <v-layout align-center justify-center :class="[ $vuetify.breakpoint.xsOnly ? 'shrink mt-4': '']">
            <v-flex xs12  sm5  class="text-center">
                <p class="body-1 mb-1" v-language:inner>courses_check_mistakes</p>
                <p class="body-1 mb-1" v-language:inner>courses_remove_spaces</p>
                <p class="body-1 mb-1" v-language:inner>university_find_similar</p>
            </v-flex>
        </v-layout>
        <v-layout align-center justify-center :class="[ $vuetify.breakpoint.xsOnly ? 'shrink mt-5': '']">
            <v-flex shrink xs6  sm3 class="text-center">
                <button @click="goTocreation()" class="min-width shrink outline d-flex align-center justify-center py-2 px-3">
                    <span class="font-weight-bold text-capitalize" v-language:inner>university_btn_create_university</span>
                </button>
            </v-flex>
            <v-flex shrink xs5  sm3 class="text-center">
                <button @click="closeDialog()" class="min-width solid d-flex align-center justify-center py-2 px-3">
                    <span class="font-weight-bold text-capitalize" v-language:inner>courses_try_again</span>
                </button>
            </v-flex>
        </v-layout>
    </v-card>
    
</template>

<script>
    import { mapGetters, mapActions } from 'vuex';
    import thinkIcon from  '../images/thinking.svg'
    export default {
        name: "verifyUniversityCreation",
        components:{ thinkIcon },
        methods: {
            ...mapActions(['updateUniVerification', 'changeUniCreateDialogState']),
           goTocreation(){
               this.updateUniVerification(true)
           },
           closeDialog(){
               this.changeUniCreateDialogState(false);
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
            border-radius: 16px;
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
            border-radius: 16px;
            color: @color-white;
            background-color: @global-purple;
        }
    }

</style>