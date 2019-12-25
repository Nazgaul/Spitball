<template>
        <v-layout class="courses-section mt-3" >
            <v-flex xs12>
                <v-card class="px-4 py-4 courses-card-wrap">
                    <v-layout>
                        <v-flex xs11 sm11  grow>
                            <div>
                                <div v-show="isTutorProfile" class="courses-title  subheading font-weight-bold mb-4" v-language:inner>profile_courses_can_help</div>
                                <div v-show="!isTutorProfile" class="courses-title  subheading font-weight-bold mb-4" v-language:inner>profile_courses_empty_state_title</div>
                            </div>
                        </v-flex>
                        <v-flex class="text-xs-right">
                            <v-icon @click="openSetClasses()" v-if="isMyProfile" class="subheading pr-2 edit-icon">sbf-edit-icon</v-icon>
                        </v-flex>
                    </v-layout>
                    <v-layout row wrap >
                        <!--<transition-group name="fade"  tag="v-layout" style="flex-direction: row; flex-wrap: wrap;">-->
                        <v-flex xs12 sm6 v-for="(course, index) in userCourses" :key="index" class="course-name">
                            <template v-if="index < showQuantity">
                                <router-link event @click.native.prevent="goToCourse(course.name)" :to="{name: 'tutors', query: {Course: course.name}}" class="cursor-pointer elevation-0 border py-3 text-truncate course-card" :class="{'mr-0': index%2}" key="two">
                                    <div class="course-name" :class="{'mr-5': index % 2 === 0}">{{course.name}}</div>
                                </router-link>
                            </template>
                        </v-flex>
                        <!--</transition-group>-->
                        <v-flex xs12 sm6 v-if="userCourses.length > showQuantity" class="course-name show-more">
                            <v-card :class="{'mr-0': $vuetify.breakpoint.smAndUp}" class="elevation-0 border  py-3" @click="expanded ? showLess() : showAll()">
                                <span class="font-weight-bold course-name">
                                    <span v-show="!expanded">
                                        <span> {{moreQuantity}}&nbsp;</span>
                                    <span v-if="moreQuantity > 1" v-language:inner>profile_expand_more_courses</span>
                                        <span v-else v-language:inner>profile_expand_more_single_course</span>
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
    import { mapGetters, mapMutations } from 'vuex';
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
            ...mapMutations(["UPDATE_SEARCH_LOADING"]),
            showAll() {
                this.showQuantity = this.userCourses.length;
                this.expanded = true;
            },
            showLess(){
                this.showQuantity = this.defaultToShow;
                this.expanded = false;
            },
            openSetClasses(){
                if(this.isTutorProfile){
                    this.$router.push({name: 'editCourse'});
                }else{
                    this.$router.push({name: 'addCourse'});
                }
            },
            goToCourse(courseName){
                this.UPDATE_SEARCH_LOADING(true);
                this.$router.push({name: 'feed', query: {Course: courseName}})
            },
        },
        computed: {
            ...mapGetters(['getProfile', 'isTutorProfile']),
            userCourses() {
                if(this.getProfile &&  this.getProfile.about && this.getProfile.about.courses){
                    return this.getProfile.about.courses;
                }
                return [];

            },
            moreQuantity(){
                if(this.userCourses.length > this.showQuantity){
                    return this.userCourses.length - this.showQuantity
                }
                return null
            },
        },
    }
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';

    .courses-section {
        .courses-card-wrap{
            box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.18);
            border-radius: 4px;
        }
        .course-card{
            margin-left: 1px;
            margin-right: 24px;
        }
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
            color: @global-purple;
        }
        .border{
            border-top: 1px solid #f0f0f7;

        }
        .course-name {
            .giveMeEllipsisOne();
            font-size: 14px;
            color: @global-blue;
            cursor: pointer;
        }
        .show-more{
            cursor: pointer;
        }

    }

</style>