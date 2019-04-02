<template>
    <v-card class="verify-creation-wrap">
        <v-layout align-center justify-center>
            <v-flex xs12 sm12 md12 class="text-xs-center">
                <v-icon>sbf-face-icon</v-icon>
            </v-flex>
        </v-layout>
        <v-layout align-center justify-center>
            <v-flex class="text-xs-center">
                <span class="font-weight-bold heading-text">Before Creating a New Course, Try:</span>
            </v-flex>
        </v-layout>
        <v-layout align-center justify-center>
            <v-flex xs5 md5  class="text-xs-center">
                <p class="body-1 mb-1">
                    Checking for spelling mistakes
                </p>
                <p class="body-1 mb-1">
                    Add or remove spaces or special characters
                </p>
                <p class="body-1 mb-1">
                    Find similar course names
                </p>
            </v-flex>
        </v-layout>
        <v-layout align-center justify-center>
            <v-flex shrink xs6 md3 sm3>
                <button @click="goTocreation()" class="min-width shrink outline d-flex align-center justify-center py-2 px-3">
                    <span class="font-weight-bold text-capitalize">Create Course</span>
                </button>
            </v-flex>
            <v-flex shrink xs6 md2 sm2>
                <button @click="closeDialog()" class="min-width solid d-flex align-center justify-center py-2 px-3">
                    <span class="font-weight-bold text-capitalize">Try Again</span>
                </button>
            </v-flex>
        </v-layout>
    </v-card>
    
</template>

<script>
    import { mapGetters, mapActions } from 'vuex';
    export default {
        name: "verifyCreation",
        computed: {

        },
        methods: {
            ...mapActions(['updateVerification', 'changeCreateDialogState']),
           goTocreation(){
               this.updateVerification(true)
           },
           closeDialog(){
               this.changeCreateDialogState(false)
           }
        },
    };
</script>

<style lang="less">
    @import '../../../styles/mixin.less';
    .verify-creation-wrap{
        min-height: 340px;
        .outline{
            border-radius: 16px;
            border: solid 1px @profileTextColor;
            color: @profileTextColor;
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
            background-color: @profileTextColor;
        }
    }

</style>