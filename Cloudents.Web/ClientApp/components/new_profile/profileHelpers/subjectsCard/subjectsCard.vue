<template>
        <v-layout  class="subjects-section mt-4">
            <v-flex xs12>
                <v-card class="px-4 py-4">
                    <v-layout>
                        <v-flex xs12 sm12 md12>
                            <div>
                                <div class="subject-title mb-4" v-language:inner>profile_subjects</div>
                            </div>
                        </v-flex>
                        <v-flex class="text-xs-right">
                            <v-icon class="edit-icon subheading pr-2">sbf-edit-icon</v-icon>
                        </v-flex>
                    </v-layout>
                    <v-layout row wrap>
                        <v-flex xs12 sm6 md6 v-for="singleSubject in subjects" class="subject-name">
                            <v-card class="elevation-0 border py-3" :class="{'mr-3': $vuetify.breakpoint.smAndUp}">
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
        .edit-icon{
            color: @purpleLight;
            opacity: 0.41;
            font-size: 20px;
            cursor: pointer;
            @media(max-width: @screen-xs){
                color: @purpleDark;
            }
        }
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