<template>
        <v-layout class="aboutme-section">
            <v-flex xs12>
                <v-card class="px-4 pt-4">
                    <v-layout column>
                        <v-flex xs12 sm12 md12>
                            <div>
                                <div class="about-title mb-2">Who am I?</div>
                            </div>
                            <div class="mt-2">
                             <p class="about-text">{{aboutMeText | truncate(isOpen, '...', textLimit)}}
                             </p>
                            </div>
                        </v-flex>
                        <v-flex class="read-more-action mt-2 mb-2">
                            <v-divider style="width: 100%; height: 2px;"></v-divider>
                            <a class="read-more-text pt-3 pb-1" @click="isOpen = !isOpen">
                                <span v-if="!isOpen">Read more...</span>
                                <span v-else>Read Less</span>
                            </a>
                        </v-flex>
                    </v-layout>
                </v-card>
            </v-flex>
        </v-layout>
</template>

<script>
    export default {
        name: "tutorAboutMe",
        data() {
            return {
                textLimit: 150,
                defOpen: false

            }
        },
        props: {
            aboutMeText: {
                type: String,
                default: ' I am a 2nd year Physics Student at the University of Oxford. I was lucky to have the support of great teachers and talented older students especially when I was learning new Maths, and understand firsthand the value of having someone patiently listen to the topics and questions you are struggling with and taki,  I am a 2nd year Physics Student at the University of Oxford. I was lucky to have the support of great teachers and talented older students especially when I was learning new Maths, and understand firsthand the value of having someone patiently listen to the topics and questions you are struggling with and taki'
            },
        },
        computed: {
            isOpen :{
                get(){
                    return this.defOpen
                },
                set(val){
                    this.defOpen = val
                }

            }
        },
        filters: {
            truncate(val, isOpen, suffix, textLimit){
                if (val.length > textLimit && !isOpen) {
                    return val.substring(0, textLimit) + suffix;
                } else {
                    return val;
                }

            }
        }
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
        .read-more-action{
            text-align: center;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            .read-more-text{
                font-family: @fontOpenSans;
                font-size: 12px;
                font-weight: 600;
                line-height: 1.31;
                color: @color-blue-new;
            }

        }
        .about-text{
            font-size: 18px;
            line-height: 1.59;
            color: @textColor;
        }
    }

</style>