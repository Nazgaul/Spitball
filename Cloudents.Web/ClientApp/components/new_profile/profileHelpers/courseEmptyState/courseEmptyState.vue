<template>
    <v-layout class="courses-empty-container">
        <v-flex xs12>
            <v-card class="px-4 py-4 elevation-0 empty-card">
                <v-layout align-start justify-start >
                    <v-flex xs12    class="text-xs-left">
                            <div class="courses-empty-title mb-4" v-language:inner>profile_courses_empty_state_title</div>
                        <v-divider></v-divider>
                    </v-flex>
                </v-layout>
                <v-layout align-center justify-center class="pt-4">
                    <v-flex xs2 sm1 >
                            <v-icon class="empty-course-icon" >sbf-courses-icon</v-icon>
                    </v-flex>
                    <v-flex xs10 sm8  class="text-xs-left">
                        <span class="empty-text" v-language:inner>profile_courses_empty_select_courses</span>
                    </v-flex>
                </v-layout>
                <v-layout class="pt-4">
                    <v-flex>
                        <v-btn @click="openUniInterface()"  rounded outline class="select-btn text-uppercase">
                            <span v-language:inner>profile_courses_empty_btn</span>
                        </v-btn>
                    </v-flex>
                </v-layout>
            </v-card>
        </v-flex>
    </v-layout>
</template>

<script>
    export default {
        name: "courseEmptyState",
        methods: {
            openUniInterface(){
                this.$router.push({name:'addCourse'});
            }
        },

    }
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';
    .courses-empty-container{
        width: 100%;
        max-width: 760px;
        .empty-card{
            /*box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.24)!important;*/
            border-radius: 4px;
        }
        .courses-empty-title{
            font-size: 18px;
            font-weight: bold;
            color: @global-purple;
        }
        .empty-course-icon{
            font-size: 28px;
            color: @global-purple;
        }
        .empty-text{
            font-size: 12px;
            color: @textColor;
        }
        .select-btn{
            font-size: 12px;
            color: @global-purple;
            font-weight: 600;
        }
    }

</style>