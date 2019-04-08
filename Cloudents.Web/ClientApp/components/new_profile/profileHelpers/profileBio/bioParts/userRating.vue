<template>
        <v-layout align-center justify-center class="rating-container">
            <v-flex>
                <v-rating
                        v-model="rating"
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
                   <span :style="{ color: rateNumColor }" class="caption">
       {{ rating }}
      </span>
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