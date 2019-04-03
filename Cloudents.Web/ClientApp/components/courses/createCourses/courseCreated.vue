<template>
    <v-card class="course-created-wrap pt-4">

            <v-layout class="close-toolbar"  :class="[$vuetify.breakpoint.xsOnly ? 'column align-center pb-4 mx-3 shrink': 'row mx-4 pb-3']">
                <v-flex xs12 sm1 md1 order-xs2 order-sm1 order-md1 class="mr-2" :class="{'mb-4 mr-0': $vuetify.breakpoint.xsOnly}">
                    <v-icon class="check-icon">sbf-check-circle</v-icon>
                </v-flex>
                <v-flex d-flex xs12 order-xs3 order-sm2 order-md2>
                    <v-layout column>
                        <v-flex d-flex :class="{'shrink text-xs-center': $vuetify.breakpoint.xsOnly}">
                            <span class="created-heading">You've Created</span>
                        </v-flex>
                        <v-flex d-flex>
                            <span class="font-weight-bold subheading">{{courseName}}</span>
                        </v-flex>
                    </v-layout>
                </v-flex>
                <v-flex order-xs1 order-sm3 order-md3 xs12  class="text-xs-right" :class="[$vuetify.breakpoint.xsOnly ? 'shrink align-self-end pt-2': '']">
                    <v-icon @click="closeDialog()" class="subheading course-close-icon font-weight-thin">sbf-close</v-icon>
                </v-flex>
            </v-layout>

        <v-layout align-center justify-center column class="mb-2 mt-4" :class="[ $vuetify.breakpoint.xsOnly ? 'mt-5 shrink': '']">
            <v-flex  class="text-xs-center"  :class="[ $vuetify.breakpoint.xsOnly ? 'shrink mt-1': '']">
                <span class="font-weight-bold title">Invite classmates</span>
            </v-flex>
            <v-flex>
                <span class="small-text">Sub Title for adding classmates</span>
            </v-flex>
        </v-layout>
        <v-layout align-center justify-center class="mt-4 mb-3" column :class="{'shrink mt-5 pt-2': $vuetify.breakpoint.xsOnly}">
            <v-flex xs6 md12 sm12 class="text-xs-center pb-2">
                <button @click="copyClassLink()" class="min-width solid d-flex align-center justify-center py-3 px-3">
                    <span class="font-weight-bold btn-text text-capitalize">Copy Course Link</span>
                </button>
            </v-flex>
            <v-flex xs12 md12 sm12 class="text-xs-center">
                <span @click="goToCoursesList()" class="caption blue-text cursor-pointer">Back to My Courses</span>
            </v-flex>
        </v-layout>
        <v-layout align-center justify-center>
            <v-flex shrink xs12 md12 sm12 class="text-xs-center">
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
            courseName: ''
        },
        methods: {
            ...mapActions(['changeCreateDialogState']),
            goToCoursesList() {
                this.$router.push({name: 'editCourse'});
                this.closeDialog();
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
            color: @colorBlue;
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
            background-color: @profileTextColor;
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