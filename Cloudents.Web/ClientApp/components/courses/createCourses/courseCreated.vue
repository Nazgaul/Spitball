<template>
    <v-card class="course-created-wrap pt-4">
        <v-layout class="close-toolbar ml-4 mr-4 pb-3" align-center justify-center row wrap>
            <v-flex xs12 shrink class="mr-2 text-xs-center text-sm-left">
                <v-icon class="check-icon">sbf-check-circle</v-icon>
            </v-flex>
            <v-flex xs12 sm10 md10 grow class="text-xs-center text-sm-left">
                <v-layout column>
                    <v-flex>
                        <span class="created-heading">You've Created</span>
                    </v-flex>
                    <v-flex xs12 class="text-xs-center text-sm-left">
                        <span class="font-weight-bold subheading">{{courseName}}</span>
                    </v-flex>
                </v-layout>
            </v-flex>
            <v-flex class="text-xs-right">
                <v-icon @click="closeDialog()" class="subheading course-close-icon font-weight-thin">sbf-close</v-icon>
            </v-flex>
        </v-layout>

        <v-layout align-center justify-center column class="mb-2 mt-4">
            <v-flex  class="text-xs-center">
                <span class="font-weight-bold title">Invite classmates</span>
            </v-flex>
            <v-flex >
                <span class="small-text">Sub Title for adding classmates</span>
            </v-flex>
        </v-layout>
        <v-layout align-center justify-center class="mt-4 mb-3" column>
            <v-flex xs12 md12 sm12 class="text-xs-center pb-2">
                <button @click="copyClassLink()" class="min-width solid d-flex align-center justify-center py-3 px-3">
                    <span class="font-weight-bold btn-text text-capitalize">Copy Course Link</span>
                </button>
            </v-flex>
            <v-flex xs12 md12 sm12 class="text-xs-center">
                <span @click="goToCoursesList()" class="caption blue-text">Back to My Courses</span>
            </v-flex>
        </v-layout>
        <v-layout align-center justify-center>
            <v-flex shrink xs12 md12 sm12 class="text-xs-center">
                <img class="bottom-image" src="../images/courses-empty-image.png" alt="">
            </v-flex>
        </v-layout>
    </v-card>
</template>

<script>
    import { mapActions } from 'vuex';
    export default {
        name: "courseCreated",
        data() {
            return {
                isCopied: false
            };

        },
        props: {
            courseName: ''
        },
        methods: {
            ...mapActions(['changeCreateDialogState']),
            goToCoursesList() {
                this.$router.push({name: 'editCourse'});
            },
            copyClassLink() {
                let url = global.location.href;
                let self = this;
                self.$copyText(url).then((e) => {
                    self.isCopied = true;
                }, (e) => {
                });
                setTimeout((uncopy) => {
                    self.isCopied = false;
                }, 1000);

            },
            closeDialog(){
                this.changeCreateDialogState(false)
            }
        },

    };
</script>

<style lang="less">
    @import '../../../styles/mixin.less';
    .course-created-wrap {
        .blue-text{
            color: @colorBlue;
        }
        .bottom-image{
            max-width: 266px;
        }
        .close-toolbar{
            border-bottom: 1px solid rgba(67, 66, 93, 0.17);
            @media(max-width: @screen-xs){
                max-height: 54px;
                margin-top: 12px;
            }
        }
        .created-heading{
            font-size: 14px;
            color: @colorBlackNew;
        }
        .solid {
            outline: none;
            border-radius: 24px;
            background-color: @profileTextColor;
            .btn-text {
                color: lighten(@color-white, 87%);
            }
        }
        .check-icon {
            color: #a3a0fb;
            font-size: 38px;
        }
        .small-text {
            font-size: 14px;
            color: @colorBlackNew;
        }
    }

</style>