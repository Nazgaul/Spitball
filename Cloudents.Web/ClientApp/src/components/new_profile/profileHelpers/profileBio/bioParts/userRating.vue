<template>
        <v-layout align-center justify-center class="rating-container">
            <v-flex shrink :class="{'me-1': $vuetify.breakpoint.smAndUp}">
                <v-rating :class="['ratingCmp']"
                        v-model="dynamicRating"
                        :color="starColor"
                        :background-color="starColor"
                        :length="ratingLength"
                        half-icon="sbf-star-rating-half"
                        full-icon="sbf-star-rating-full"
                        empty-icon="sbf-star-rating-empty"
                        half-increments
                        :readonly="readonly"
                        :size="size"
                        :hover="true" />
            </v-flex>
            <v-flex>
                <div>
                   <span v-show="showRateNumber" :style="{ color: rateNumColor }" class="caption ms-1 pb-1 rating-number">
       {{ dynamicRating }}
      </span>
                </div>
            </v-flex>
        </v-layout>
</template>

<script>
    export default {
        name: "userReviews",
        props: {
            callbackFn: {
                type: Function,
                required: false
            },
            ratingLength: {
                type: Number,
                default: 5
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
                 if(this.rating > 0){
                    return Number.parseFloat(Number.parseFloat(this.rating).toFixed(2));
                 }else{
                    return this.rating
                 }
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
        .ratingCmp{
            display: flex;
            .sbf-star-rating-half{
                -moz-transform: none /*rtl:scaleX(-1)*/;
                -o-transform: none/*rtl:scaleX(-1)*/;
                -webkit-transform: none/*rtl:scaleX(-1)*/;
                transform: none/*rtl:scaleX(-1)*/;
                height: inherit;
            }
        }
        .v-rating{
            .v-icon{
                padding: 0;
            }
        }
    }

</style>