<template>
    <div class="review-submitted-wrap">
        <v-layout row wrap align-center justify-center class="pb-4">
            <v-flex xs12   class="text-center pt-4">
                <v-icon class="star-icon">sbf-star-rating-full</v-icon>
            </v-flex>
            <v-flex xs12   class="text-center pt-4">
                <span class="font-weight-black final-review-title" v-language:inner>leaveReview_final_title</span>
            </v-flex>
            <v-flex xs12   class="text-center pt-3 pb-5">
                <span @click="goToReview()" class="body-2 blue-text cursor-pointer" v-language:inner>leaveReview_final_goTo</span>
            </v-flex>
        </v-layout>
    </div>
</template>

<script>
    export default {
        name: "finalScreen",
        props: {
            tutorId: {
                type: Number,
                default: 0
            },
            tutorName:{
                type: String,
                default: ''
            }
        },
        methods: {
            goToReview() {
                this.$router.push(
                    {name: 'profile', params: {id: this.tutorId, name:this.tutorName}}
                );
            }
        },
    };
</script>

<style lang="less">
    @import '../../../../../styles/mixin.less';
    .review-submitted-wrap{
        .final-review-title{
            font-size: 18px;
            color: @global-purple;
        }
        .star-icon{
            font-size: 86px;
            color: #ffca54;
        }
        .blue-text{
            color: @global-blue;
        }
    }
</style>