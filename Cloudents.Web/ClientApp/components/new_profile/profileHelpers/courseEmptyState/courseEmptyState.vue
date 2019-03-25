<template>
    <v-layout class="courses-empty-container">
        <v-flex xs12>
            <v-card class="px-4 py-4 elevation-0 empty-card">
                <v-layout align-start justify-start >
                    <v-flex xs12 sm12 md12  class="text-xs-left">
                            <div class="courses-empty-title mb-4">My Courses</div>
                        <v-divider></v-divider>
                    </v-flex>
                </v-layout>
                <v-layout align-center justify-center class="pt-4">
                    <v-flex xs1 sm1 md1>
                            <v-icon class="empty-course-icon">sbf-courses-icon</v-icon>
                    </v-flex>
                    <v-flex xs8 sm8 md8 class="text-xs-left">
                        <span class="empty-text">Selecting your courses will help  you get personilised study materials and tutors</span>
                    </v-flex>
                </v-layout>
                <v-layout class="pt-3">
                    <v-flex>
                        <v-btn  round outline class="select-btn text-uppercase">Select my courses</v-btn>
                    </v-flex>
                </v-layout>
            </v-card>
        </v-flex>
    </v-layout>
</template>

<script>
    export default {
        name: "courseEmptyState",

    }
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';
    .courses-empty-container{
        width: 100%;
        max-width: 760px;
        .empty-card{
            box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.24)!important;
            border-radius: 4px;
        }
        .courses-empty-title{
            font-size: 18px;
            font-weight: bold;
            color: @profileTextColor;
        }
        .empty-course-icon{
            font-size: 28px;
            color: @profileTextColor;
        }
        .empty-text{
            font-family: @fontOpenSans;
            font-size: 12px;
            color: @textColor;
        }
        .select-btn{
            font-size: 12px;
            color: @profileTextColor;
            font-weight: 600;
        }
    }

</style>