<template>
        <v-layout  class="subjects-section mt-4">
            <v-flex xs12>
                <v-card class="px-4 py-4">
                    <v-layout>
                        <v-flex xs12 sm12 md12>
                            <div>
                                <div class="subject-title mb-4">Subjects</div>
                            </div>
                        </v-flex>
                    </v-layout>
                    <v-layout row wrap>
                        <v-flex xs6 v-for="singleSubject in subjects" class="subject-name">
                            <v-card class="elevation-0 border mr-3 py-3">
                                <span class="subject-name">{{singleSubject}}</span>
                            </v-card>
                        </v-flex>

                    </v-layout>
                </v-card>
            </v-flex>
        </v-layout>
</template>

<script>
    export default {
        name: "subjectsCard",
        data() {
            return {
                subjects: [
                    'Basic Physics',
                    'Analytical Mechanics',
                    'Biological Physics',
                    'Quantum Mechanics A',
                    'Quantum Mechanics A',
                    'Methods of Mathematical Physics',
                    'Methods of Mathematical Physics',
                    'Methods of Mathematical Physics',
                    'Methods of Mathematical Physics',
                    'Methods of Mathematical Physics',
                    'Methods of Mathematical Physics',

                ]
            }
        },
    }
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';

    .subjects-section {
        .subject-title {
            font-size: 18px;
            font-weight: bold;
            color: @profileTextColor;
        }
        .border{
            border-top: 1px solid rgba(0, 0, 0, 0.24);
        }
        .subject-name {
            font-size: 14px;
            color: @color-blue-new;
        }

    }

</style>