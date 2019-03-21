<template>
        <v-layout class="aboutme-section">
            <v-flex xs12>
                <v-card class="px-4 py-4">
                    <v-layout>
                        <v-flex xs12 sm12 md12>
                            <div>
                                <div class="about-title mb-2">Who am I?</div>
                            </div>
                            <div class="mt-2">
                             <p class="about-text">
                                 I am a 2nd year Physics Student at the University of Oxford. I was lucky to have the support of great teachers and talented older students especially when I was learning new Maths, and understand firsthand the value of having someone patiently listen to the topics and questions you are struggling with and taki…
                             </p>
                            </div>
                        </v-flex>
                    </v-layout>
                </v-card>
            </v-flex>
        </v-layout>
</template>

<script>
    export default {
        name: "tutorAboutMe"
    }
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';
    .aboutme-section{
        .about-title{
            height: 18px;
            font-size: 18px;
            font-weight: bold;
            line-height: 1;
            color: @profileTextColor;
        }
        .about-text{
            font-size: 18px;
            line-height: 1.59;
            color: @textColor;
        }
    }

</style>