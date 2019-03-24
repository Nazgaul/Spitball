<template>
        <v-layout class="courses-section mt-4">
            <v-flex xs12>
                <v-card class="px-4 py-4">
                    <v-layout>
                        <v-flex xs12 sm12 md12>
                            <div>
                                <div class="courses-title mb-4">Courses I Can Help You With</div>
                            </div>
                        </v-flex>
                    </v-layout>
                    <v-layout row wrap>
                        <v-flex transition="slide-y-transition" xs12 sm6 md6
                                v-for="(course, index) in aboutData"
                                v-if="index < showQuantity"
                                :key="index" class="course-name">
                            <v-card class="elevation-0 border mr-3 py-3">
                                <span class="course-name">{{course.name}}</span>
                            </v-card>
                        </v-flex>
                        <v-flex xs12 sm6 md6 v-if="aboutData.length >= showQuantity" class="course-name show-more">
                            <v-card class="elevation-0 border mr-3 py-3" @click="expanded ? showLess() : showAll()">
                                <span class="font-weight-bold course-name">
                                    <span v-show="!expanded">{{moreQuantity}} More Courses</span>
                                    <span v-show="expanded">Show Less</span>
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
        methods: {
            showAll() {
                this.showQuantity = this.aboutData.length;
                this.expanded = true;
            },
            showLess(){
                this.showQuantity = this.defaultToShow;
                this.expanded = false;
            }
        },
        computed: {
            ...mapGetters(['getProfile']),
            aboutData() {
                if(this.getProfile && this.getProfile.about){
                    return this.getProfile.about;
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
        }
        .show-more{
            cursor: pointer;
        }

    }

</style>