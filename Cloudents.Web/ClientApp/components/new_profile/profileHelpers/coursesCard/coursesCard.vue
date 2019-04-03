<template>
        <v-layout class="courses-section mt-4" >
            <v-flex xs12>
                <v-card class="px-4 py-4">
                    <v-layout>
                        <v-flex xs11 sm11 md11 grow>
                            <div>
                                <div class="courses-title mb-4" v-language:inner>profile_courses_can_help</div>
                            </div>
                        </v-flex>
                        <v-flex class="text-xs-right">
                            <v-icon @click="openSetClasses()" v-if="isMyProfile" class="subheading pr-2 edit-icon">sbf-edit-icon</v-icon>
                        </v-flex>
                    </v-layout>
                    <v-layout row wrap >
                        <!--<transition-group name="fade"  tag="v-layout" style="flex-direction: row; flex-wrap: wrap;">-->
                        <v-flex  xs12 sm6 md6 key="sdf"
                                v-for="(course, index) in aboutData"
                                v-if="index < showQuantity"
                                :key="index" class="course-name">
                            <v-card class="elevation-0 border mr-3 py-3 text-truncate" key="wqerghfh">
                                <span class="course-name">{{course.name}}</span>
                            </v-card>
                        </v-flex>
                        <!--</transition-group>-->
                        <v-flex xs12 sm6 md6 v-if="aboutData.length >= showQuantity" class="course-name show-more">
                            <v-card :class="{'mr-3': $vuetify.breakpoint.smAndUp}" class="elevation-0 border  py-3" @click="expanded ? showLess() : showAll()">
                                <span class="font-weight-bold course-name">
                                    <span v-show="!expanded">
                                        <span> {{moreQuantity}}&nbsp;</span>
                                    <span v-language:inner>profile_expand_more_courses</span>
                                    </span>
                                    <span v-show="expanded" v-language:inner>profile_expand_less</span>
                                </span>
                            </v-card>
                        </v-flex>
                    </v-layout>
                </v-card>
            </v-flex>
        </v-layout>
</template>

<script>
    import { mapGetters } from 'vuex';
    export default {
        name: "coursesCard",
        data() {
            return {
                showQuantity: 5,
                defaultToShow: 5,
                expanded: false
            }
        },
        props: {
            isMyProfile: {
                type: Boolean,
                default: false
            },
        },
        methods: {
            showAll() {
                this.showQuantity = this.aboutData.length;
                this.expanded = true;
            },
            showLess(){
                this.showQuantity = this.defaultToShow;
                this.expanded = false;
            },
            openSetClasses(){
                this.$router.push({
                    name:'uniselect',
                    params: {step: 'setClass'}
                });
            }
        },
        computed: {
            ...mapGetters(['getProfile']),
            aboutData() {
                if(this.getProfile &&  this.getProfile.about && this.getProfile.about.courses){
                    return this.getProfile.about.courses;
                }
                return [];

            },
            moreQuantity(){
                if(this.aboutData.length > this.showQuantity){
                    return this.aboutData.length - this.showQuantity
                }
            },
        },

    }
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';

    .courses-section {
        .edit-icon{
                color: @purpleLight;
                opacity: 0.41;
                font-size: 20px;
                cursor: pointer;
                @media(max-width: @screen-xs){
                    color: @purpleDark;
            }
        }
        .courses-title {
            font-size: 18px;
            font-weight: bold;
            color: @profileTextColor;
        }
        .border{
            border-top: 1px solid rgba(0, 0, 0, 0.24);
        }
        .course-name {
            font-size: 14px;
            color: @color-blue-new;
            cursor: pointer;
        }
        .show-more{
            cursor: pointer;
        }

    }

</style>