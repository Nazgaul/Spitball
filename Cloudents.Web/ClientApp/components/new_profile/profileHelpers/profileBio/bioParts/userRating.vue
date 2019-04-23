<template>
        <v-layout align-center justify-center class="rating-container">
            <v-flex shrink class="mr-2">
                <v-rating
                        v-model="dynamicRating"
                        :color="starColor"
                        :background-color="starColor"
                        :length="ratingLength"
                        half-icon="sbf-star-rating-half"
                        full-icon="sbf-star-rating-full"
                        empty-icon="sbf-star-rating-empty"
                        half-increments
                        :readonly="readonly"
                        hover
                        :size="size"
                ></v-rating>
            </v-flex>
            <v-flex>
                <div class="mb-1">
                   <span v-show="showRateNumber" :style="{ color: rateNumColor }" class="caption ml-1 pb-1 rating-number">
       {{ rating }}
      </span>
                </div>
            </v-flex>
        </v-layout>
</template>

<script>
    export default {
        name: "userReviews",
        data() {
            return {
                ratingLength: 5
            }
        },
        props: {
            callbackFn: {
                type: Function,
                required: false
            },
            starColor: {
                type: String,
                default: '#ffca54'
            },
            readonly:{
                type: Boolean,
                default: true
            },
            size:{
                type: String,
                default: '24'
            },
            rateNumColor:{
                type: String,
                default: '#ffffff'
            },
            rating: {
                type: Number,
                default: 0
            },
            showRateNumber:{
                type: Boolean,
                default: true
            }
        },
        computed: {
            dynamicRating:{
             get(){
                 return this.rating
             },
            set(newValue){
                if(this.callbackFn){
                    this.callbackFn(newValue)
                }
            }
            }
        },
    }
</script>

<style lang="less">
    .rating-container{
        .v-rating{
            .v-icon{
                padding: 0;
            }
        }
    }

</style>