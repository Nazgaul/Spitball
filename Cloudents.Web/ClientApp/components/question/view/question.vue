<template>
    <div class="answer-question">

        <!-- Web version -->
        <v-container v-if="!$vuetify.breakpoint.xsOnly">
            <v-layout row wrap>

                <v-flex xs12 class="breadcrumbs">
                    <a class="ask-question" href="/askquestion">
                        <v-icon>sbf-comment</v-icon>
                        Ask a question</a>
                    <span class="question-category">Questions  / Algebra</span>
                </v-flex>

                <v-flex xs7>
                    <question-thread :questionId="id"/>
                </v-flex>

                <v-flex xs4>
                            <div ref="chat-area"></div>
                               
                </v-flex>

            </v-layout>
        </v-container>

        <!-- Mobile version with tabs -->
        <div v-else>
            <v-tabs grow>

                <v-tabs-bar>
                    <v-tabs-slider color="blue"></v-tabs-slider>
                    <v-tabs-item :href="'#tab-1'" :key="'1'">Question</v-tabs-item>
                    <v-tabs-item :href="'#tab-2'" :key="'2'">Chat with questioner</v-tabs-item>
                </v-tabs-bar>

                <v-tabs-items>

                    <v-tabs-content :key="'1'" :id="'tab-1'" class="tab-padding">
                        <v-flex xs12>
                            <question-thread :questionId="id"/>
                        </v-flex>
                    </v-tabs-content>

                    <v-tabs-content :key="'2'" :id="'tab-2'">
                        <v-flex xs12>
                            <mini-chat></mini-chat>
                        </v-flex>
                    </v-tabs-content>

                </v-tabs-items>

            </v-tabs>
        </div>
    </div>
</template>

<style src="./question.less" lang="less"></style>
<script src="./question.js"></script>
