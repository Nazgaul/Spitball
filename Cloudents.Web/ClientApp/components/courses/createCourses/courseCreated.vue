<template>
    <v-card class="course-created-wrap pt-4">

            <v-layout class="close-toolbar"  :class="[$vuetify.breakpoint.xsOnly ? 'column align-center pb-4 mx-3 shrink': 'mx-4 pb-4']">
                <v-flex xs12 sm1  order-xs2 order-sm1  class="mr-2" :class="{'mb-4 mr-0': $vuetify.breakpoint.xsOnly}">
                    <v-icon class="check-icon">sbf-check-circle</v-icon>
                </v-flex>
                <v-flex d-flex xs12 order-xs3 order-sm2 >
                    <v-layout column>
                        <v-flex d-flex :class="{'shrink text-center': $vuetify.breakpoint.xsOnly}">
                            <span class="created-heading" v-t>courses_you_created</span>
                        </v-flex>
                        <v-flex d-flex>
                            <span class="font-weight-bold subtitle-1">{{courseName}}</span>
                        </v-flex>
                    </v-layout>
                </v-flex>
                <v-flex order-xs1 order-sm3  xs12  class="text-xs-right" :class="[$vuetify.breakpoint.xsOnly ? 'shrink align-self-end pt-2': '']">
                    <v-icon @click="closeDialog()" class="subtitle-1 course-close-icon font-weight-thin">sbf-close</v-icon>
                </v-flex>
            </v-layout>

        <v-layout align-center justify-center column class="mb-2 mt-4" :class="[ $vuetify.breakpoint.xsOnly ? 'mt-5 shrink': '']">
            <v-flex  class="text-center"  :class="[ $vuetify.breakpoint.xsOnly ? 'shrink mt-1': '']">
                <span class="font-weight-bold title" v-t>courses_invite_classmates</span>
            </v-flex>
            <v-flex>
                <!-- <span class="small-text" v-t>courses_sub_title_invite</span> -->
            </v-flex>
        </v-layout>
        <v-layout align-center justify-center class="mt-4 mb-4" column :class="{'shrink mt-5 pt-2': $vuetify.breakpoint.xsOnly}">
            <v-flex xs6  sm12 class="text-center pb-2">
                <button @click="copyClassLink()" class="min-width solid d-flex align-center justify-center">
                    <v-icon class="pr-0" color="white" transition="fade-transition"
                            v-show="isCopied">sbf-checkmark
                    </v-icon>
                    <span class="font-weight-bold btn-text text-capitalize" v-show="!isCopied" v-t>courses_copy_link</span>
                    <span class="font-weight-bold btn-text text-capitalize" v-show="isCopied" v-t>courses_link_copied</span>


                </button>
            </v-flex>
            <v-flex xs12  sm12 class="text-center">
                <span @click="goToCoursesList()" class="caption blue-text cursor-pointer" v-t>courses_back_to_list</span>
            </v-flex>
        </v-layout>
        <v-layout align-center justify-center>
            <v-flex shrink xs12  sm12 class="text-center">
                <img class="bottom-image" src="../images/created-course.png" alt="created new course">
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
            courseName: {
                type: String,
                default: ''
            }
        },
        methods: {
            ...mapActions(['changeCreateDialogState']),
            goToCoursesList() {
                this.$router.push({name: 'editCourse'});
                this.closeDialog();
            },
            copyClassLink() {
                let url = `${global.location.origin}/feed/?Course=${this.courseName}`;
                let self = this;
                self.$copyText(url).then(() => {
                    self.isCopied = true;
                });
                setTimeout(() => {
                    self.isCopied = false;
                }, 2000);

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
    .course-created-wrap {
        .blue-text{
            color: @global-blue;
        }
        .bottom-image{
            max-width: 266px;
        }
        .close-toolbar{
            border-bottom: 1px solid rgba(67, 66, 93, 0.17);
        }
        .created-heading{
            font-size: 14px;
            color: @colorBlackNew;
        }
        .solid {
            outline: none;
            border-radius: 24px;
            background-color: @global-purple;
            min-width: 168px;
            height: 48px;
            padding-right: 32px;
            padding-left: 24px;
            @media(max-width: @screen-xs){
                min-width: 168px;
            }
            .btn-text {
                color: lighten(@color-white, 87%);
            }
        }
        .check-icon {
            color: #a3a0fb;
            font-size: 42px;
        }
        .small-text {
            font-size: 14px;
            color: @colorBlackNew;
        }
    }

</style>