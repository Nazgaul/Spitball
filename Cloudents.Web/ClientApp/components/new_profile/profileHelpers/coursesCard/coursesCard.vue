<template>
    <v-container class="courses-section">
        <v-layout>
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
                        <v-flex xs6 v-for="course in courses" class="course-name">
                            <v-card class="elevation-0 border mr-3 py-3">
                                <span class="course-name">{{course}}</span>
                            </v-card>
                        </v-flex>

                    </v-layout>
                </v-card>
            </v-flex>

        </v-layout>
    </v-container>
</template>

<script>
    export default {
        name: "coursesCard",
        data() {
            return {
                courses: ['Basic Physics', 'Analytical Mechanics', 'Biological Physics', 'Quantum Mechanics A', 'Quantum Mechanics A',]
            }
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
            font-size: 16px;
            color: @color-blue-new;
        }

    }

</style>