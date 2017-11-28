<template>
    <v-container class="features" grid-list-xs>
        <v-layout row justify-space-around wrap>
            <v-flex v-for="(feature, index) in features" class="feature" md4 xs12 pa-3>
                <v-layout class="wrapper" row align-start justify-space-between d-flex wrap>
                    <v-flex class="icon-wrapper" md12>
                        <component class="item" :is="'feature-'+index"></component>
                    </v-flex>
                    <v-flex md12>
                        <div class="data text-xs-left text-sm-center">
                            <h3>{{feature.title}}</h3>
                            <p class="hidden-xs-only">{{feature.text}}</p>
                            <p class="hidden-sm-and-up">{{feature.mobileText}}</p>
                        </div>
                    </v-flex>
                </v-layout>
            </v-flex>
        </v-layout>
    </v-container>
</template>


<style src="./features.less" lang="less" scoped></style>
<script>
    import featureDocument from "./svg/feature-documents.svg";
    import featureHomework from "./svg/feature-homework.svg";
    import featureTextbook from "./svg/feature-textbook.svg";

    export let features =
        {

            document: {
                title: "Millions of Documents",
                text: "Find study guides, homework, and notes for courses at your school.",
                mobileText: "study guides and notes."
            },

            textbook: {
                title: "Save up to 50% on Textbooks",
                text: "A new course load doesn’t have to break the bank! Find the best prices to rent, buy, or sell your textbooks.",
                mobileText: "Best prices to rent, buy, or sell."
            },

            homework: {
                title: "Homework Help 24/7",
                text: "Find the answers to all your questions with step-by-step help from expert tutors, in person or online.",
                mobileText: "Step-by-step help from expert tutors."
            },
        }
    export default {
        data() {
            return {
                features: features
            }
        },
        components: {
            featureDocument,
            featureHomework,
            featureTextbook
        }
    }
</script>